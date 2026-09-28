#include <inttypes.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

#include "CmllEoMoves.generated.h"

#define CORNER_PERMUTATIONS 40320u
#define CORNER_ORIENTATIONS 2187u
#define CORNER_STATES (CORNER_PERMUTATIONS * CORNER_ORIENTATIONS)
#define CMLL_CORNER_ORBIT 3674160u /* 7! * 3^6: DBL is fixed by U/R/r/F. */
#define BLOCK_EDGE_PLACEMENTS 665280u /* 12 P 6 */
#define BLOCK_EDGE_STATES (BLOCK_EDGE_PLACEMENTS * 64u)
#define CMLL_BLOCK_EDGE_ORBIT 80640u
#define EO_CENTER_STATES (2048u * 4u)
#define CMLL_EO_CENTER_ORBIT 2048u
#define TRANSPOSITION_BITS 23u
#define TRANSPOSITION_SIZE (1u << TRANSPOSITION_BITS)
#define TRANSPOSITION_PROBES 16u
#define REVERSE_TABLE_BITS 24u
#define REVERSE_TABLE_SIZE (1u << REVERSE_TABLE_BITS)

typedef struct {
    uint8_t edge_piece[12], edge_flip[12];
    uint8_t corner_piece[8], corner_twist[8];
    uint8_t center_piece[6];
} cube_state;

typedef struct {
    uint64_t edge_key;
    uint32_t corner_key;
    uint16_t first_moves;
    uint8_t depth;
    uint8_t generation;
} transposition_entry;

typedef struct {
    uint64_t edge_key;
    uint32_t corner_key;
    uint8_t distance;
    uint8_t next_move;
    uint8_t occupied;
    uint8_t settled;
} reverse_entry;

typedef struct {
    uint32_t *items;
    size_t count;
    size_t capacity;
} index_bucket;

typedef struct {
    uint8_t *corner;
    uint8_t *block_edge;
    uint8_t *eo_center;
    uint8_t *lse;
    uint64_t nodes;
    unsigned path[64];
    unsigned solution[64];
    unsigned solution_length;
    cube_state batch_initial;
    uint8_t batch_permutations[360][6];
    uint8_t batch_cost[360];
    uint16_t batch_first_moves[360];
    unsigned batch_solved;
    transposition_entry *transpositions;
    uint8_t transposition_generation;
    reverse_entry *reverse;
} search_context;

static const uint8_t lse_positions[6] = {2, 0, 1, 3, 5, 7}; /* UL UR UF UB DF DB */
static const uint8_t lse_pieces[6] = {2, 0, 1, 3, 5, 7};

static unsigned orientation_rank(const uint8_t *twists);
static void orientation_unrank(unsigned rank, uint8_t *twists);
static unsigned orientation12_rank(const uint8_t *flips);
static void orientation12_unrank(unsigned rank, uint8_t *flips);
static unsigned corner_state_index(const cube_state *state);
static uint64_t mix64(uint64_t value);
static uint8_t *read_lse_distances(const char *path);
static cube_state solved_cube(void);

static uint8_t center_reference[4][6];

static inline void apply_move(cube_state *state, unsigned move) {
    cube_state next;
    for (unsigned destination = 0; destination < 12; ++destination) {
        unsigned source = cmll_eo_edge_source[move][destination];
        next.edge_piece[destination] = state->edge_piece[source];
        next.edge_flip[destination] = state->edge_flip[source] ^ cmll_eo_edge_flip[move][destination];
    }
    for (unsigned destination = 0; destination < 8; ++destination) {
        unsigned source = cmll_eo_corner_source[move][destination];
        next.corner_piece[destination] = state->corner_piece[source];
        next.corner_twist[destination] = (state->corner_twist[source] + cmll_eo_corner_twist[move][destination]) % 3;
    }
    for (unsigned destination = 0; destination < 6; ++destination) {
        next.center_piece[destination] = state->center_piece[cmll_eo_center_source[move][destination]];
    }
    *state = next;
}

static uint64_t monotonic_ns(void) {
    struct timespec value;
    if (clock_gettime(CLOCK_MONOTONIC, &value) != 0) {
        perror("clock_gettime");
        exit(2);
    }
    return (uint64_t)value.tv_sec * UINT64_C(1000000000) + (uint64_t)value.tv_nsec;
}

static void validate_moves(void) {
    for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
        uint64_t seen = 0;
        for (unsigned destination = 0; destination < 54; ++destination) {
            unsigned source = cmll_eo_facelet_source[move][destination];
            if (source >= 54 || (seen & (UINT64_C(1) << source)) != 0) {
                fprintf(stderr, "Move %s is not a facelet permutation.\n", cmll_eo_move_names[move]);
                exit(1);
            }
            seen |= UINT64_C(1) << source;
        }
    }
}

static void initialize_center_references(void) {
    for (unsigned index = 0; index < 6; ++index) center_reference[0][index] = (uint8_t)index;
    for (unsigned offset = 1; offset < 4; ++offset) {
        for (unsigned destination = 0; destination < 6; ++destination) {
            center_reference[offset][destination] = center_reference[offset - 1][cmll_eo_center_source[6][destination]];
        }
    }
}

static unsigned center_offset(const cube_state *state) {
    for (unsigned offset = 0; offset < 4; ++offset) {
        if (memcmp(state->center_piece, center_reference[offset], 6) == 0) return offset;
    }
    fprintf(stderr, "State left the M-axis center orbit.\n");
    exit(1);
}

static unsigned permutation_rank(const uint8_t *permutation, unsigned count) {
    unsigned rank = 0;
    for (unsigned index = 0; index < count; ++index) {
        unsigned smaller = 0;
        for (unsigned later = index + 1; later < count; ++later) smaller += permutation[later] < permutation[index];
        rank = rank * (count - index) + smaller;
    }
    return rank;
}

static void permutation_unrank(unsigned rank, unsigned count, uint8_t *permutation) {
    static const unsigned factorial[] = {1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600};
    uint8_t available[12];
    for (unsigned index = 0; index < count; ++index) available[index] = (uint8_t)index;
    unsigned available_count = count;
    for (unsigned index = 0; index < count; ++index) {
        unsigned block = factorial[count - index - 1];
        unsigned digit = rank / block;
        rank %= block;
        permutation[index] = available[digit];
        memmove(&available[digit], &available[digit + 1], available_count - digit - 1);
        --available_count;
    }
}

static void unpack_state(uint64_t edge_key, uint32_t corner_key, cube_state *state) {
    unsigned edge_permutation = (unsigned)(edge_key & ((UINT64_C(1) << 29) - 1));
    unsigned edge_orientation = (unsigned)((edge_key >> 29) & 2047u);
    unsigned center = (unsigned)((edge_key >> 40) & 3u);
    unsigned corner_permutation = corner_key / CORNER_ORIENTATIONS;
    unsigned corner_orientation = corner_key % CORNER_ORIENTATIONS;
    permutation_unrank(edge_permutation, 12, state->edge_piece);
    orientation12_unrank(edge_orientation, state->edge_flip);
    permutation_unrank(corner_permutation, 8, state->corner_piece);
    orientation_unrank(corner_orientation, state->corner_twist);
    memcpy(state->center_piece, center_reference[center], 6);
}

static void pack_state(const cube_state *state, uint64_t *edge_key, uint32_t *corner_key) {
    *edge_key = permutation_rank(state->edge_piece, 12) |
                ((uint64_t)orientation12_rank(state->edge_flip) << 29) |
                ((uint64_t)center_offset(state) << 40);
    *corner_key = corner_state_index(state);
}

static void bucket_push(index_bucket *bucket, uint32_t value) {
    if (bucket->count == bucket->capacity) {
        size_t next_capacity = bucket->capacity == 0 ? 1024 : bucket->capacity * 2;
        uint32_t *next = realloc(bucket->items, next_capacity * sizeof *next);
        if (!next) { fprintf(stderr, "Unable to grow reverse-search bucket.\n"); exit(2); }
        bucket->items = next;
        bucket->capacity = next_capacity;
    }
    bucket->items[bucket->count++] = value;
}

static uint32_t reverse_find_slot(reverse_entry *table, uint64_t edge_key, uint32_t corner_key, int *found) {
    uint64_t hash = mix64(edge_key ^ ((uint64_t)corner_key << 17));
    uint32_t slot = (uint32_t)hash & (REVERSE_TABLE_SIZE - 1u);
    for (uint32_t probe = 0; probe < REVERSE_TABLE_SIZE; ++probe) {
        reverse_entry *entry = &table[(slot + probe) & (REVERSE_TABLE_SIZE - 1u)];
        if (!entry->occupied) { *found = 0; return (slot + probe) & (REVERSE_TABLE_SIZE - 1u); }
        if (entry->edge_key == edge_key && entry->corner_key == corner_key) { *found = 1; return (slot + probe) & (REVERSE_TABLE_SIZE - 1u); }
    }
    fprintf(stderr, "Reverse-search hash table is full.\n");
    exit(2);
}

static int is_post_sb_boundary(const cube_state *state) {
    static const uint8_t block_edges[6] = {4, 6, 8, 9, 10, 11};
    for (unsigned index = 0; index < 6; ++index) {
        unsigned position = block_edges[index];
        if (state->edge_piece[position] != position || state->edge_flip[position] != 0) return 0;
    }
    for (unsigned position = 4; position < 8; ++position) {
        if (state->corner_piece[position] != position || state->corner_twist[position] != 0) return 0;
    }
    return state->center_piece[2] == 2 && state->center_piece[3] == 3;
}

static reverse_entry *build_reverse_wave(const char *lse_path, unsigned maximum_cost) {
    static const unsigned inverse_move[CMLLEO_MOVE_COUNT] = {1,0,2,4,3,5,7,6,8,10,9,11};
    if (maximum_cost >= 64) { fprintf(stderr, "Prototype reverse-wave maximum cost must be below 64.\n"); exit(2); }
    initialize_center_references();
    uint8_t *lse = read_lse_distances(lse_path);
    reverse_entry *table = calloc(REVERSE_TABLE_SIZE, sizeof *table);
    index_bucket buckets[64] = {{0}};
    if (!table) { fprintf(stderr, "Unable to allocate reverse-search table (256 MiB).\n"); exit(2); }

    uint64_t discovered = 0, settled = 0, boundary = 0;
    unsigned sources = 0;
    for (unsigned lse_index = 0; lse_index < 368640; ++lse_index) {
        if (lse[lse_index] == UINT8_MAX) continue;
        unsigned auf = lse_index % 4u;
        unsigned center_and_above = lse_index / 4u;
        unsigned center_m = center_and_above % 4u;
        unsigned flip_and_above = center_and_above / 4u;
        unsigned flip = flip_and_above % 32u;
        unsigned permutation_rank_value = flip_and_above / 32u;
        if (flip != 0 || (center_m & 1u) != 0) continue;
        cube_state source = solved_cube();
        uint8_t permutation[6];
        permutation_unrank(permutation_rank_value, 6, permutation);
        for (unsigned position = 0; position < 6; ++position) source.edge_piece[lse_positions[position]] = lse_pieces[permutation[position]];
        memcpy(source.center_piece, center_reference[(4u - center_m) & 3u], 6);
        for (unsigned turn = 0; turn < auf; ++turn) apply_move(&source, 0);
        uint64_t edge_key; uint32_t corner_key;
        pack_state(&source, &edge_key, &corner_key);
        int found;
        uint32_t slot = reverse_find_slot(table, edge_key, corner_key, &found);
        unsigned distance = lse[lse_index];
        if (!found || distance < table[slot].distance) {
            if (!found) { table[slot].occupied = 1; table[slot].edge_key = edge_key; table[slot].corner_key = corner_key; ++discovered; }
            table[slot].distance = (uint8_t)distance;
            table[slot].next_move = UINT8_MAX;
            bucket_push(&buckets[distance], slot);
        }
        ++sources;
    }
    if (sources != 2880) { fprintf(stderr, "Expected 2,880 weighted CMLLEO terminal sources, found %u.\n", sources); exit(1); }

    for (unsigned distance = 0; distance <= maximum_cost; ++distance) {
        index_bucket *bucket = &buckets[distance];
        for (size_t item = 0; item < bucket->count; ++item) {
            uint32_t slot = bucket->items[item];
            reverse_entry *entry = &table[slot];
            if (entry->settled || entry->distance != distance) continue;
            entry->settled = 1;
            ++settled;
            cube_state state;
            unpack_state(entry->edge_key, entry->corner_key, &state);
            if (is_post_sb_boundary(&state)) ++boundary;
            if (distance == maximum_cost) continue;
            for (unsigned forward_move = 0; forward_move < CMLLEO_MOVE_COUNT; ++forward_move) {
                cube_state predecessor = state;
                apply_move(&predecessor, inverse_move[forward_move]);
                uint64_t predecessor_edge; uint32_t predecessor_corner;
                pack_state(&predecessor, &predecessor_edge, &predecessor_corner);
                int found;
                uint32_t predecessor_slot = reverse_find_slot(table, predecessor_edge, predecessor_corner, &found);
                unsigned next_distance = distance + 1;
                if (!found || next_distance < table[predecessor_slot].distance) {
                    if (!found) {
                        if (discovered * 10 >= REVERSE_TABLE_SIZE * 8) {
                            fprintf(stderr, "Reverse-search table reached 80%% occupancy at cost %u; increase REVERSE_TABLE_BITS.\n", distance);
                            exit(2);
                        }
                        table[predecessor_slot].occupied = 1;
                        table[predecessor_slot].edge_key = predecessor_edge;
                        table[predecessor_slot].corner_key = predecessor_corner;
                        ++discovered;
                    }
                    table[predecessor_slot].distance = (uint8_t)next_distance;
                    table[predecessor_slot].next_move = (uint8_t)forward_move;
                    bucket_push(&buckets[next_distance], predecessor_slot);
                }
            }
        }
        printf("CMLLEO_REVERSE_WAVE|cost=%u|settled=%" PRIu64 "|discovered=%" PRIu64 "|boundary=%" PRIu64 "\n", distance, settled, discovered, boundary);
        fflush(stdout);
    }
    for (unsigned distance = 0; distance < 64; ++distance) free(buckets[distance].items);
    free(lse);
    return table;
}

static unsigned orientation_rank(const uint8_t *twists) {
    unsigned rank = 0, factor = 1;
    for (unsigned index = 0; index < 7; ++index) {
        rank += twists[index] * factor;
        factor *= 3;
    }
    return rank;
}

static void orientation_unrank(unsigned rank, uint8_t *twists) {
    unsigned sum = 0;
    for (unsigned index = 0; index < 7; ++index) {
        twists[index] = (uint8_t)(rank % 3);
        sum += twists[index];
        rank /= 3;
    }
    twists[7] = (uint8_t)((3 - sum % 3) % 3);
}

static void build_corner_transitions(uint16_t (*permutation_next)[CMLLEO_MOVE_COUNT], uint16_t (*orientation_next)[CMLLEO_MOVE_COUNT]) {
    uint8_t current[8], next[8];
    for (unsigned rank = 0; rank < CORNER_PERMUTATIONS; ++rank) {
        permutation_unrank(rank, 8, current);
        for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
            for (unsigned destination = 0; destination < 8; ++destination) current[destination] &= 7;
            for (unsigned destination = 0; destination < 8; ++destination) next[destination] = current[cmll_eo_corner_source[move][destination]];
            permutation_next[rank][move] = (uint16_t)permutation_rank(next, 8);
        }
    }
    for (unsigned rank = 0; rank < CORNER_ORIENTATIONS; ++rank) {
        orientation_unrank(rank, current);
        for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
            for (unsigned destination = 0; destination < 8; ++destination) {
                next[destination] = (uint8_t)((current[cmll_eo_corner_source[move][destination]] + cmll_eo_corner_twist[move][destination]) % 3);
            }
            orientation_next[rank][move] = (uint16_t)orientation_rank(next);
        }
    }
}

static unsigned corner_next(unsigned state, unsigned move,
                            const uint16_t (*permutation_next)[CMLLEO_MOVE_COUNT],
                            const uint16_t (*orientation_next)[CMLLEO_MOVE_COUNT]) {
    unsigned permutation = state / CORNER_ORIENTATIONS;
    unsigned orientation = state % CORNER_ORIENTATIONS;
    return (unsigned)permutation_next[permutation][move] * CORNER_ORIENTATIONS + orientation_next[orientation][move];
}

static void build_corner_pdb(const char *path) {
    uint16_t (*permutation_next)[CMLLEO_MOVE_COUNT] = malloc(sizeof(*permutation_next) * CORNER_PERMUTATIONS);
    uint16_t (*orientation_next)[CMLLEO_MOVE_COUNT] = malloc(sizeof(*orientation_next) * CORNER_ORIENTATIONS);
    uint8_t *distance = malloc(CORNER_STATES);
    uint32_t *queue = malloc(sizeof(*queue) * CORNER_STATES);
    if (!permutation_next || !orientation_next || !distance || !queue) {
        fprintf(stderr, "Unable to allocate corner PDB workspace (about 430 MiB).\n");
        exit(2);
    }
    build_corner_transitions(permutation_next, orientation_next);
    memset(distance, UINT8_MAX, CORNER_STATES);

    unsigned solved = 0;
    uint64_t head = 0, tail = 0;
    unsigned current = solved;
    for (unsigned auf = 0; auf < 4; ++auf) {
        if (distance[current] == UINT8_MAX) {
            distance[current] = 0;
            queue[tail++] = current;
        }
        current = corner_next(current, 0, permutation_next, orientation_next); /* U */
    }

    unsigned maximum = 0;
    uint64_t next_report = UINT64_C(5000000);
    while (head < tail) {
        current = queue[head++];
        unsigned next_distance = (unsigned)distance[current] + 1;
        for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
            unsigned neighbor = corner_next(current, move, permutation_next, orientation_next);
            if (distance[neighbor] == UINT8_MAX) {
                distance[neighbor] = (uint8_t)next_distance;
                maximum = maximum < next_distance ? next_distance : maximum;
                queue[tail++] = neighbor;
            }
        }
        if (head >= next_report) {
            fprintf(stderr, "CMLLEO_CORNER_PROGRESS|processed=%" PRIu64 "|discovered=%" PRIu64 "|depth=%u\n", head, tail, maximum);
            next_report += UINT64_C(5000000);
        }
    }
    if (tail != CMLL_CORNER_ORBIT) {
        fprintf(stderr, "Corner PDB coverage failure: reached %" PRIu64 " states; expected %u in the U/R/r/F orbit.\n", tail, CMLL_CORNER_ORBIT);
        exit(1);
    }

    FILE *file = fopen(path, "wb");
    if (!file) { perror(path); exit(2); }
    const uint8_t magic[8] = {'R','C','E','C','P','D','B','1'};
    if (fwrite(magic, 1, sizeof magic, file) != sizeof magic ||
        fwrite(&maximum, sizeof maximum, 1, file) != 1 ||
        fwrite(distance, 1, CORNER_STATES, file) != CORNER_STATES || fclose(file) != 0) {
        perror("writing corner PDB");
        exit(2);
    }
    printf("CMLLEO_CORNER_PDB|path=%s|states=%" PRIu64 "|max=%u\n", path, tail, maximum);
    free(queue); free(distance); free(orientation_next); free(permutation_next);
}

static unsigned partial_permutation_rank(const uint8_t *positions) {
    uint8_t available[12];
    for (unsigned index = 0; index < 12; ++index) available[index] = (uint8_t)index;
    unsigned available_count = 12, rank = 0;
    for (unsigned piece = 0; piece < 6; ++piece) {
        unsigned digit = 0;
        while (digit < available_count && available[digit] != positions[piece]) ++digit;
        if (digit == available_count) { fprintf(stderr, "Invalid partial edge permutation.\n"); exit(1); }
        rank = rank * available_count + digit;
        memmove(&available[digit], &available[digit + 1], available_count - digit - 1);
        --available_count;
    }
    return rank;
}

static void partial_permutation_unrank(unsigned rank, uint8_t *positions) {
    uint8_t digits[6], available[12];
    for (int piece = 5; piece >= 0; --piece) {
        unsigned radix = 12u - (unsigned)piece;
        digits[piece] = (uint8_t)(rank % radix);
        rank /= radix;
    }
    for (unsigned index = 0; index < 12; ++index) available[index] = (uint8_t)index;
    unsigned available_count = 12;
    for (unsigned piece = 0; piece < 6; ++piece) {
        unsigned digit = digits[piece];
        positions[piece] = available[digit];
        memmove(&available[digit], &available[digit + 1], available_count - digit - 1);
        --available_count;
    }
}

static void build_block_edge_transitions(uint32_t (*placement_next)[CMLLEO_MOVE_COUNT], uint8_t (*flip_xor)[CMLLEO_MOVE_COUNT]) {
    static const uint8_t block_pieces[6] = {4, 6, 8, 9, 10, 11}; /* DR, DL, FR, FL, BL, BR */
    uint8_t destination_of_source[CMLLEO_MOVE_COUNT][12];
    for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
        for (unsigned destination = 0; destination < 12; ++destination) {
            destination_of_source[move][cmll_eo_edge_source[move][destination]] = (uint8_t)destination;
        }
    }
    uint8_t positions[6], next[6];
    for (unsigned rank = 0; rank < BLOCK_EDGE_PLACEMENTS; ++rank) {
        partial_permutation_unrank(rank, positions);
        for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
            unsigned mask = 0;
            for (unsigned piece = 0; piece < 6; ++piece) {
                unsigned destination = destination_of_source[move][positions[piece]];
                next[piece] = (uint8_t)destination;
                mask |= (unsigned)cmll_eo_edge_flip[move][destination] << piece;
            }
            placement_next[rank][move] = partial_permutation_rank(next);
            flip_xor[rank][move] = (uint8_t)mask;
        }
    }
    (void)block_pieces;
}

static void build_block_edge_pdb(const char *path) {
    uint32_t (*placement_next)[CMLLEO_MOVE_COUNT] = malloc(sizeof(*placement_next) * BLOCK_EDGE_PLACEMENTS);
    uint8_t (*flip_xor)[CMLLEO_MOVE_COUNT] = malloc(sizeof(*flip_xor) * BLOCK_EDGE_PLACEMENTS);
    uint8_t *distance = malloc(BLOCK_EDGE_STATES);
    uint32_t *queue = malloc(sizeof(*queue) * BLOCK_EDGE_STATES);
    if (!placement_next || !flip_xor || !distance || !queue) {
        fprintf(stderr, "Unable to allocate block-edge PDB workspace (about 220 MiB).\n");
        exit(2);
    }
    build_block_edge_transitions(placement_next, flip_xor);
    memset(distance, UINT8_MAX, BLOCK_EDGE_STATES);
    const uint8_t solved_positions[6] = {4, 6, 8, 9, 10, 11};
    unsigned solved = partial_permutation_rank(solved_positions) * 64u;
    uint64_t head = 0, tail = 1;
    queue[0] = solved;
    distance[solved] = 0;
    unsigned maximum = 0;
    uint64_t next_report = UINT64_C(5000000);
    while (head < tail) {
        unsigned current = queue[head++];
        unsigned placement = current / 64u, flips = current % 64u;
        unsigned next_distance = (unsigned)distance[current] + 1;
        for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
            unsigned neighbor = placement_next[placement][move] * 64u + (flips ^ flip_xor[placement][move]);
            if (distance[neighbor] == UINT8_MAX) {
                distance[neighbor] = (uint8_t)next_distance;
                maximum = maximum < next_distance ? next_distance : maximum;
                queue[tail++] = neighbor;
            }
        }
        if (head >= next_report) {
            fprintf(stderr, "CMLLEO_BLOCK_EDGE_PROGRESS|processed=%" PRIu64 "|discovered=%" PRIu64 "|depth=%u\n", head, tail, maximum);
            next_report += UINT64_C(5000000);
        }
    }
    if (tail != CMLL_BLOCK_EDGE_ORBIT) {
        fprintf(stderr, "Block-edge PDB coverage failure: reached %" PRIu64 " states; expected %u in the U/R/r/F orbit.\n", tail, CMLL_BLOCK_EDGE_ORBIT);
        exit(1);
    }
    FILE *file = fopen(path, "wb");
    if (!file) { perror(path); exit(2); }
    const uint8_t magic[8] = {'R','C','E','B','P','D','B','1'};
    if (fwrite(magic, 1, sizeof magic, file) != sizeof magic ||
        fwrite(&maximum, sizeof maximum, 1, file) != 1 ||
        fwrite(distance, 1, BLOCK_EDGE_STATES, file) != BLOCK_EDGE_STATES || fclose(file) != 0) {
        perror("writing block-edge PDB");
        exit(2);
    }
    printf("CMLLEO_BLOCK_EDGE_PDB|path=%s|states=%" PRIu64 "|sparse_slots=%u|max=%u\n", path, tail, BLOCK_EDGE_STATES, maximum);
    free(queue); free(distance); free(flip_xor); free(placement_next);
}

static unsigned orientation12_rank(const uint8_t *flips) {
    unsigned rank = 0;
    for (unsigned index = 0; index < 11; ++index) rank |= (unsigned)flips[index] << index;
    return rank;
}

static void orientation12_unrank(unsigned rank, uint8_t *flips) {
    unsigned parity = 0;
    for (unsigned index = 0; index < 11; ++index) {
        flips[index] = (uint8_t)((rank >> index) & 1u);
        parity ^= flips[index];
    }
    flips[11] = (uint8_t)parity;
}

static void build_eo_center_pdb(const char *path) {
    uint16_t orientation_next[2048][CMLLEO_MOVE_COUNT];
    uint8_t center_next[4][CMLLEO_MOVE_COUNT];
    uint8_t current[12], next[12];
    for (unsigned rank = 0; rank < 2048; ++rank) {
        orientation12_unrank(rank, current);
        for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
            for (unsigned destination = 0; destination < 12; ++destination) {
                next[destination] = current[cmll_eo_edge_source[move][destination]] ^ cmll_eo_edge_flip[move][destination];
            }
            orientation_next[rank][move] = (uint16_t)orientation12_rank(next);
        }
    }

    initialize_center_references();
    for (unsigned offset = 0; offset < 4; ++offset) {
        for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
            uint8_t moved[6];
            for (unsigned destination = 0; destination < 6; ++destination) moved[destination] = center_reference[offset][cmll_eo_center_source[move][destination]];
            unsigned found = 4;
            for (unsigned candidate = 0; candidate < 4; ++candidate) {
                if (memcmp(moved, center_reference[candidate], 6) == 0) { found = candidate; break; }
            }
            if (found == 4) { fprintf(stderr, "Move %s leaves the M-axis center orbit.\n", cmll_eo_move_names[move]); exit(1); }
            center_next[offset][move] = (uint8_t)found;
        }
    }

    uint8_t distance[EO_CENTER_STATES];
    uint16_t queue[EO_CENTER_STATES];
    memset(distance, UINT8_MAX, sizeof distance);
    uint64_t head = 0, tail = 0;
    for (unsigned center = 0; center < 4; center += 2) {
        unsigned goal = center;
        distance[goal] = 0;
        queue[tail++] = (uint16_t)goal;
    }
    unsigned maximum = 0;
    while (head < tail) {
        unsigned state = queue[head++];
        unsigned orientation = state / 4u, center = state % 4u;
        unsigned next_distance = (unsigned)distance[state] + 1;
        for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
            unsigned neighbor = (unsigned)orientation_next[orientation][move] * 4u + center_next[center][move];
            if (distance[neighbor] == UINT8_MAX) {
                distance[neighbor] = (uint8_t)next_distance;
                maximum = maximum < next_distance ? next_distance : maximum;
                queue[tail++] = (uint16_t)neighbor;
            }
        }
    }
    if (tail != CMLL_EO_CENTER_ORBIT) {
        fprintf(stderr, "EO/center PDB coverage failure: reached %" PRIu64 " states; expected %u in the coupled U/R/r/F orbit.\n", tail, CMLL_EO_CENTER_ORBIT);
        exit(1);
    }
    FILE *file = fopen(path, "wb");
    if (!file) { perror(path); exit(2); }
    const uint8_t magic[8] = {'R','C','E','O','P','D','B','1'};
    if (fwrite(magic, 1, sizeof magic, file) != sizeof magic ||
        fwrite(&maximum, sizeof maximum, 1, file) != 1 ||
        fwrite(distance, 1, sizeof distance, file) != sizeof distance || fclose(file) != 0) {
        perror("writing EO/center PDB");
        exit(2);
    }
    printf("CMLLEO_EO_CENTER_PDB|path=%s|states=%" PRIu64 "|max=%u\n", path, tail, maximum);
}

static uint8_t *read_payload(const char *path, const char magic[8], size_t size) {
    FILE *file = fopen(path, "rb");
    if (!file) { perror(path); exit(2); }
    char actual_magic[8];
    unsigned maximum;
    uint8_t *payload = malloc(size);
    if (!payload || fread(actual_magic, 1, 8, file) != 8 || memcmp(actual_magic, magic, 8) != 0 ||
        fread(&maximum, sizeof maximum, 1, file) != 1 || fread(payload, 1, size, file) != size || fgetc(file) != EOF) {
        fprintf(stderr, "Invalid pruning table %s.\n", path);
        exit(2);
    }
    fclose(file);
    return payload;
}

static uint8_t *read_lse_distances(const char *path) {
    FILE *file = fopen(path, "rb");
    if (!file) { perror(path); exit(2); }
    uint8_t magic[4];
    uint32_t version, slots, reachable, maximum;
    if (fread(magic, 1, 4, file) != 4 || memcmp(magic, "RLSE", 4) != 0 ||
        fread(&version, 4, 1, file) != 1 || fread(&slots, 4, 1, file) != 1 ||
        fread(&reachable, 4, 1, file) != 1 || fread(&maximum, 4, 1, file) != 1 ||
        version != 1 || slots != 368640u || reachable != 184320u) {
        fprintf(stderr, "Invalid exact LSE policy %s.\n", path);
        exit(2);
    }
    uint8_t *distance = malloc(slots);
    if (!distance) exit(2);
    for (unsigned index = 0; index < slots; ++index) {
        int d = fgetc(file), mask = fgetc(file);
        if (d == EOF || mask == EOF) { fprintf(stderr, "Truncated LSE policy.\n"); exit(2); }
        distance[index] = (uint8_t)d;
    }
    fclose(file);
    return distance;
}

static unsigned corner_state_index(const cube_state *state) {
    return permutation_rank(state->corner_piece, 8) * CORNER_ORIENTATIONS + orientation_rank(state->corner_twist);
}

static unsigned block_edge_state_index(const cube_state *state) {
    static const uint8_t pieces[6] = {4, 6, 8, 9, 10, 11};
    uint8_t positions[6];
    unsigned flips = 0;
    for (unsigned piece_index = 0; piece_index < 6; ++piece_index) {
        unsigned position = 0;
        while (position < 12 && state->edge_piece[position] != pieces[piece_index]) ++position;
        if (position == 12) { fprintf(stderr, "Missing block edge.\n"); exit(1); }
        positions[piece_index] = (uint8_t)position;
        flips |= (unsigned)state->edge_flip[position] << piece_index;
    }
    return partial_permutation_rank(positions) * 64u + flips;
}

static unsigned eo_center_state_index(const cube_state *state) {
    return orientation12_rank(state->edge_flip) * 4u + center_offset(state);
}

static unsigned top_corner_auf(const cube_state *state) {
    cube_state reference;
    memset(&reference, 0, sizeof reference);
    for (unsigned index = 0; index < 12; ++index) reference.edge_piece[index] = (uint8_t)index;
    for (unsigned index = 0; index < 8; ++index) reference.corner_piece[index] = (uint8_t)index;
    for (unsigned index = 0; index < 6; ++index) reference.center_piece[index] = (uint8_t)index;
    for (unsigned auf = 0; auf < 4; ++auf) {
        if (memcmp(state->corner_piece, reference.corner_piece, 4) == 0 &&
            memcmp(state->corner_twist, reference.corner_twist, 4) == 0 &&
            memcmp(state->corner_piece + 4, reference.corner_piece + 4, 4) == 0 &&
            memcmp(state->corner_twist + 4, reference.corner_twist + 4, 4) == 0) return auf;
        apply_move(&reference, 0);
    }
    return 4;
}

static int terminal_lse_index(const cube_state *state) {
    if (block_edge_state_index(state) != partial_permutation_rank((const uint8_t[]){4,6,8,9,10,11}) * 64u) return -1;
    if (orientation12_rank(state->edge_flip) != 0) return -1;
    unsigned center_r = center_offset(state);
    if ((center_r & 1u) != 0) return -1;
    unsigned auf = top_corner_auf(state);
    if (auf == 4) return -1;

    uint8_t permutation[6];
    unsigned flip_rank = 0;
    for (unsigned position = 0; position < 6; ++position) {
        uint8_t piece = state->edge_piece[lse_positions[position]];
        unsigned identity = 0;
        while (identity < 6 && lse_pieces[identity] != piece) ++identity;
        if (identity == 6) return -1;
        permutation[position] = (uint8_t)identity;
        if (position < 5) flip_rank |= (unsigned)state->edge_flip[lse_positions[position]] << position;
    }
    unsigned center_m = (4u - center_r) & 3u; /* r has the center action of M'. */
    return (int)((((permutation_rank(permutation, 6) * 32u + flip_rank) * 4u + center_m) * 4u + auf));
}

static int permutation_parity(const uint8_t *permutation, unsigned count) {
    unsigned inversions = 0;
    for (unsigned left = 0; left < count; ++left) {
        for (unsigned right = left + 1; right < count; ++right) inversions += permutation[left] > permutation[right];
    }
    return (int)(inversions & 1u);
}

static void initialize_batch_permutations(search_context *context) {
    uint8_t indices[6] = {0, 1, 2, 3, 4, 5};
    unsigned count = 0;
    for (unsigned rank = 0; rank < 720; ++rank) {
        permutation_unrank(rank, 6, indices);
        if (permutation_parity(indices, 6) != 0) continue;
        for (unsigned position = 0; position < 6; ++position) {
            context->batch_permutations[count][position] = context->batch_initial.edge_piece[lse_positions[indices[position]]];
        }
        ++count;
    }
    if (count != 360) { fprintf(stderr, "Expected 360 parity-compatible edge permutations.\n"); exit(1); }
    memset(context->batch_cost, UINT8_MAX, sizeof context->batch_cost);
    memset(context->batch_first_moves, 0, sizeof context->batch_first_moves);
    context->batch_solved = 0;
}

static int terminal_components(const cube_state *initial, const cube_state *state, unsigned *auf, unsigned *center_m, uint8_t source_slot[6]) {
    if (block_edge_state_index(state) != partial_permutation_rank((const uint8_t[]){4,6,8,9,10,11}) * 64u) return 0;
    if (orientation12_rank(state->edge_flip) != 0) return 0;
    unsigned center_r = center_offset(state);
    if ((center_r & 1u) != 0) return 0;
    *auf = top_corner_auf(state);
    if (*auf == 4) return 0;
    *center_m = (4u - center_r) & 3u;
    for (unsigned destination = 0; destination < 6; ++destination) {
        uint8_t piece = state->edge_piece[lse_positions[destination]];
        unsigned source = 0;
        while (source < 6 && initial->edge_piece[lse_positions[source]] != piece) ++source;
        if (source == 6) return 0;
        source_slot[destination] = (uint8_t)source;
    }
    return 1;
}

static unsigned heuristic(const cube_state *state, const search_context *context) {
    unsigned corner = context->corner[corner_state_index(state)];
    unsigned block = context->block_edge[block_edge_state_index(state)];
    unsigned eo = context->eo_center[eo_center_state_index(state)];
    if (corner == UINT8_MAX || block == UINT8_MAX || eo == UINT8_MAX) {
        fprintf(stderr, "Search state is outside a validated pruning orbit.\n");
        exit(1);
    }
    unsigned result = corner > block ? corner : block;
    return result > eo ? result : eo;
}

static uint64_t mix64(uint64_t value) {
    value ^= value >> 30;
    value *= UINT64_C(0xbf58476d1ce4e5b9);
    value ^= value >> 27;
    value *= UINT64_C(0x94d049bb133111eb);
    return value ^ (value >> 31);
}

static int transposition_prune(search_context *context, const cube_state *state, unsigned depth, int previous_family) {
    unsigned edge_permutation = permutation_rank(state->edge_piece, 12);
    unsigned edge_orientation = orientation12_rank(state->edge_flip);
    uint64_t edge_key = edge_permutation | ((uint64_t)edge_orientation << 29) |
                        ((uint64_t)center_offset(state) << 40) | ((uint64_t)(previous_family + 1) << 42);
    uint32_t corner_key = corner_state_index(state);
    uint16_t first_moves = depth == 0 ? UINT16_C(0x8000) : (uint16_t)(1u << context->path[0]);
    uint64_t hash = mix64(edge_key ^ ((uint64_t)corner_key << 17));
    unsigned slot = (unsigned)hash & (TRANSPOSITION_SIZE - 1u);
    for (unsigned probe = 0; probe < TRANSPOSITION_PROBES; ++probe) {
        transposition_entry *entry = &context->transpositions[(slot + probe) & (TRANSPOSITION_SIZE - 1u)];
        if (entry->generation != context->transposition_generation) {
            entry->edge_key = edge_key;
            entry->corner_key = corner_key;
            entry->first_moves = first_moves;
            entry->depth = (uint8_t)depth;
            entry->generation = context->transposition_generation;
            return 0;
        }
        if (entry->edge_key != edge_key || entry->corner_key != corner_key) continue;
        if (entry->depth < depth) return 1;
        if (entry->depth == depth) return 1; /* Deterministic one-optimal-move policy. */
        entry->depth = (uint8_t)depth;
        entry->first_moves = first_moves;
        return 0;
    }
    return 0; /* A saturated probe window costs speed, never correctness. */
}

static void next_transposition_generation(search_context *context) {
    if (!context->transpositions) return;
    if (++context->transposition_generation == 0) {
        memset(context->transpositions, 0, sizeof(*context->transpositions) * TRANSPOSITION_SIZE);
        context->transposition_generation = 1;
    }
}

static int ida(search_context *context, const cube_state *state, unsigned depth, unsigned bound, int previous_family) {
    ++context->nodes;
    unsigned lower = heuristic(state, context);
    if (depth + lower > bound) return 0;
    if (context->transpositions && transposition_prune(context, state, depth, previous_family)) return 0;
    int lse_index = terminal_lse_index(state);
    if (lse_index >= 0 && context->lse[lse_index] != UINT8_MAX && depth + context->lse[lse_index] <= bound) {
        context->solution_length = depth;
        memcpy(context->solution, context->path, sizeof(unsigned) * depth);
        return 1;
    }
    if (depth == bound) return 0;
    for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
        int family = (int)(move / 3);
        if (family == previous_family) continue;
        if (previous_family == 2 && family == 1) continue; /* R and r commute: canonical R before r. */
        cube_state next = *state;
        apply_move(&next, move);
        context->path[depth] = move;
        if (ida(context, &next, depth + 1, bound, family)) return 1;
    }
    return 0;
}

static reverse_entry *reverse_lookup(reverse_entry *table, const cube_state *state) {
    uint64_t edge_key; uint32_t corner_key;
    pack_state(state, &edge_key, &corner_key);
    int found;
    uint32_t slot = reverse_find_slot(table, edge_key, corner_key, &found);
    return found && table[slot].settled ? &table[slot] : NULL;
}

static int ida_reverse(search_context *context, const cube_state *state, unsigned depth, unsigned bound, int previous_family, uint8_t *first_move) {
    ++context->nodes;
    if (depth + heuristic(state, context) > bound) return 0;
    reverse_entry *meeting = reverse_lookup(context->reverse, state);
    if (meeting && depth + meeting->distance <= bound) {
        *first_move = depth == 0 ? meeting->next_move : (uint8_t)context->path[0];
        return 1;
    }
    if (context->transpositions && transposition_prune(context, state, depth, previous_family)) return 0;
    if (depth == bound) return 0;
    for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
        int family = (int)(move / 3);
        if (family == previous_family) continue;
        if (previous_family == 2 && family == 1) continue;
        cube_state next = *state;
        apply_move(&next, move);
        context->path[depth] = move;
        if (ida_reverse(context, &next, depth + 1, bound, family, first_move)) return 1;
    }
    return 0;
}

static void score_batch_terminal(search_context *context, const cube_state *state, unsigned depth, unsigned bound) {
    unsigned auf, center_m;
    uint8_t source_slot[6];
    if (!terminal_components(&context->batch_initial, state, &auf, &center_m, source_slot)) return;
    for (unsigned candidate = 0; candidate < 360; ++candidate) {
        if (context->batch_cost[candidate] != UINT8_MAX && context->batch_cost[candidate] < bound) continue;
        uint8_t permutation[6];
        for (unsigned destination = 0; destination < 6; ++destination) {
            uint8_t piece = context->batch_permutations[candidate][source_slot[destination]];
            unsigned identity = 0;
            while (identity < 6 && lse_pieces[identity] != piece) ++identity;
            if (identity == 6) { fprintf(stderr, "Terminal transformation introduced a block edge into LSE.\n"); exit(1); }
            permutation[destination] = (uint8_t)identity;
        }
        unsigned lse_index = (((permutation_rank(permutation, 6) * 32u) * 4u + center_m) * 4u + auf);
        unsigned lse_distance = context->lse[lse_index];
        if (lse_distance == UINT8_MAX) { fprintf(stderr, "Batch terminal produced an unreachable LSE state.\n"); exit(1); }
        unsigned cost = depth + lse_distance;
        if (cost > bound) continue;
        uint16_t first_move = depth == 0 ? 0 : (uint16_t)(1u << context->path[0]);
        if (context->batch_cost[candidate] == UINT8_MAX) {
            if (cost < bound) { fprintf(stderr, "Batch search discovered cost %u after its completed bound.\n", cost); exit(1); }
            context->batch_cost[candidate] = (uint8_t)cost;
            context->batch_first_moves[candidate] = first_move;
            ++context->batch_solved;
        } else if (cost < context->batch_cost[candidate]) {
            fprintf(stderr, "Batch search violated monotonic optimality: %u < %u.\n", cost, context->batch_cost[candidate]);
            exit(1);
        } else if (cost == context->batch_cost[candidate]) {
            context->batch_first_moves[candidate] |= first_move;
        }
    }
}

static void batch_dfs(search_context *context, const cube_state *state, unsigned depth, unsigned bound, int previous_family) {
    ++context->nodes;
    if (depth + heuristic(state, context) > bound) return;
    if (transposition_prune(context, state, depth, previous_family)) return;
    score_batch_terminal(context, state, depth, bound);
    if (depth == bound) return;
    for (unsigned move = 0; move < CMLLEO_MOVE_COUNT; ++move) {
        int family = (int)(move / 3);
        if (family == previous_family) continue;
        if (previous_family == 2 && family == 1) continue;
        cube_state next = *state;
        apply_move(&next, move);
        context->path[depth] = move;
        batch_dfs(context, &next, depth + 1, bound, family);
    }
}

static cube_state solved_cube(void) {
    cube_state state;
    memset(&state, 0, sizeof state);
    for (unsigned index = 0; index < 12; ++index) state.edge_piece[index] = (uint8_t)index;
    for (unsigned index = 0; index < 8; ++index) state.corner_piece[index] = (uint8_t)index;
    for (unsigned index = 0; index < 6; ++index) state.center_piece[index] = (uint8_t)index;
    return state;
}

static void solve_sample(const char *directory, const char *lse_path) {
    char path[1024];
    search_context context = {0};
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-corners-v1.pdb", directory);
    context.corner = read_payload(path, "RCECPDB1", CORNER_STATES);
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-block-edges-v1.pdb", directory);
    context.block_edge = read_payload(path, "RCEBPDB1", BLOCK_EDGE_STATES);
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-eo-center-v1.pdb", directory);
    context.eo_center = read_payload(path, "RCEOPDB1", EO_CENTER_STATES);
    context.lse = read_lse_distances(lse_path);
    initialize_center_references();

    cube_state state = solved_cube();
    /* Inverse Sune followed by a deterministic LSE scramble.  M = r' R. */
    const unsigned sample[] = {3,2,4,1,3,1,4, 7,3,0,4,6,2,7,3,1};
    for (unsigned index = 0; index < sizeof sample / sizeof sample[0]; ++index) apply_move(&state, sample[index]);
    unsigned initial = heuristic(&state, &context);
    uint64_t begin = monotonic_ns();
    unsigned bound = initial;
    while (!ida(&context, &state, 0, bound, -1)) {
        fprintf(stderr, "CMLLEO_IDA_BOUND|bound=%u|nodes=%" PRIu64 "\n", bound, context.nodes);
        ++bound;
    }
    cube_state terminal = state;
    for (unsigned index = 0; index < context.solution_length; ++index) apply_move(&terminal, context.solution[index]);
    int lse_index = terminal_lse_index(&terminal);
    if (lse_index < 0 || context.lse[lse_index] == UINT8_MAX || context.solution_length + context.lse[lse_index] != bound) {
        fprintf(stderr, "Weighted IDA* returned an invalid terminal or score.\n");
        exit(1);
    }
    double seconds = (double)(monotonic_ns() - begin) / 1e9;
    printf("CMLLEO_SAMPLE|combined=%u|cmll_eo=%u|remaining_lse=%u|nodes=%" PRIu64 "|seconds=%.6f|algorithm=", bound, context.solution_length, context.lse[lse_index], context.nodes, seconds);
    for (unsigned index = 0; index < context.solution_length; ++index) printf("%s%s", index ? " " : "", cmll_eo_move_names[context.solution[index]]);
    printf("\n");
    free(context.lse); free(context.eo_center); free(context.block_edge); free(context.corner);
}

static void solve_batch_sample(const char *directory, const char *lse_path) {
    char path[1024];
    search_context context = {0};
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-corners-v1.pdb", directory);
    context.corner = read_payload(path, "RCECPDB1", CORNER_STATES);
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-block-edges-v1.pdb", directory);
    context.block_edge = read_payload(path, "RCEBPDB1", BLOCK_EDGE_STATES);
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-eo-center-v1.pdb", directory);
    context.eo_center = read_payload(path, "RCEOPDB1", EO_CENTER_STATES);
    context.lse = read_lse_distances(lse_path);
    context.transpositions = calloc(TRANSPOSITION_SIZE, sizeof *context.transpositions);
    if (!context.transpositions) { fprintf(stderr, "Unable to allocate transposition table.\n"); exit(2); }
    initialize_center_references();

    context.batch_initial = solved_cube();
    const unsigned sample[] = {3,2,4,1,3,1,4, 7,3,0,4,6,2,7,3,1};
    for (unsigned index = 0; index < sizeof sample / sizeof sample[0]; ++index) apply_move(&context.batch_initial, sample[index]);
    initialize_batch_permutations(&context);

    unsigned bound = heuristic(&context.batch_initial, &context);
    const unsigned shared_bound_limit = 16;
    uint64_t begin = monotonic_ns();
    while (context.batch_solved != 360 && bound <= shared_bound_limit) {
        next_transposition_generation(&context);
        uint64_t before = context.nodes;
        batch_dfs(&context, &context.batch_initial, 0, bound, -1);
        fprintf(stderr, "CMLLEO_BATCH_BOUND|bound=%u|solved=%u/360|nodes=%" PRIu64 "\n", bound, context.batch_solved, context.nodes - before);
        ++bound;
    }

    unsigned tail_count = 0;
    for (unsigned candidate = 0; candidate < 360; ++candidate) {
        if (context.batch_cost[candidate] != UINT8_MAX) continue;
        cube_state state = context.batch_initial;
        for (unsigned position = 0; position < 6; ++position) {
            state.edge_piece[lse_positions[position]] = context.batch_permutations[candidate][position];
        }
        context.solution_length = 0;
        unsigned individual_bound = shared_bound_limit + 1;
        while (1) {
            next_transposition_generation(&context);
            if (ida(&context, &state, 0, individual_bound, -1)) break;
            ++individual_bound;
        }
        context.batch_cost[candidate] = (uint8_t)individual_bound;
        context.batch_first_moves[candidate] = context.solution_length == 0 ? 0 : (uint16_t)(1u << context.solution[0]);
        ++context.batch_solved;
        ++tail_count;
        if (tail_count % 10 == 0 || context.batch_solved == 360) {
            fprintf(stderr, "CMLLEO_BATCH_TAIL|completed=%u|solved=%u/360|last_cost=%u\n", tail_count, context.batch_solved, individual_bound);
        }
    }

    unsigned minimum = UINT8_MAX, maximum = 0;
    uint64_t sum = 0;
    for (unsigned candidate = 0; candidate < 360; ++candidate) {
        if (context.batch_cost[candidate] == UINT8_MAX) { fprintf(stderr, "Batch ended with uncovered edge permutation %u.\n", candidate); exit(1); }
        if (context.batch_cost[candidate] < minimum) minimum = context.batch_cost[candidate];
        if (context.batch_cost[candidate] > maximum) maximum = context.batch_cost[candidate];
        sum += context.batch_cost[candidate];
    }
    double seconds = (double)(monotonic_ns() - begin) / 1e9;
    printf("CMLLEO_BATCH_SAMPLE|states=360|min=%u|max=%u|average=%.6f|tail=%u|nodes=%" PRIu64 "|seconds=%.6f\n",
           minimum, maximum, (double)sum / 360.0, tail_count, context.nodes, seconds);
    free(context.transpositions); free(context.lse); free(context.eo_center); free(context.block_edge); free(context.corner);
}

static void solve_reverse_batch_sample(const char *directory, const char *lse_path, unsigned reverse_cost) {
    char path[1024];
    search_context context = {0};
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-corners-v1.pdb", directory);
    context.corner = read_payload(path, "RCECPDB1", CORNER_STATES);
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-block-edges-v1.pdb", directory);
    context.block_edge = read_payload(path, "RCEBPDB1", BLOCK_EDGE_STATES);
    snprintf(path, sizeof path, "%s/rouxlab-cmll-eo-eo-center-v1.pdb", directory);
    context.eo_center = read_payload(path, "RCEOPDB1", EO_CENTER_STATES);
    context.lse = read_lse_distances(lse_path);
    context.transpositions = calloc(TRANSPOSITION_SIZE, sizeof *context.transpositions);
    if (!context.transpositions) { fprintf(stderr, "Unable to allocate transposition table.\n"); exit(2); }
    initialize_center_references();
    context.reverse = build_reverse_wave(lse_path, reverse_cost);

    context.batch_initial = solved_cube();
    const unsigned sample[] = {3,2,4,1,3,1,4, 7,3,0,4,6,2,7,3,1};
    for (unsigned index = 0; index < sizeof sample / sizeof sample[0]; ++index) apply_move(&context.batch_initial, sample[index]);
    initialize_batch_permutations(&context);

    uint64_t begin = monotonic_ns();
    for (unsigned candidate = 0; candidate < 360; ++candidate) {
        cube_state state = context.batch_initial;
        for (unsigned position = 0; position < 6; ++position) state.edge_piece[lse_positions[position]] = context.batch_permutations[candidate][position];
        unsigned bound = heuristic(&state, &context);
        uint8_t first_move = UINT8_MAX;
        while (1) {
            next_transposition_generation(&context);
            if (ida_reverse(&context, &state, 0, bound, -1, &first_move)) break;
            ++bound;
        }
        context.batch_cost[candidate] = (uint8_t)bound;
        context.batch_first_moves[candidate] = first_move == UINT8_MAX ? 0 : (uint16_t)(1u << first_move);
        if ((candidate + 1) % 60 == 0) fprintf(stderr, "CMLLEO_REVERSE_BATCH|solved=%u/360|last_cost=%u\n", candidate + 1, bound);
    }
    unsigned minimum = UINT8_MAX, maximum = 0;
    uint64_t sum = 0;
    for (unsigned candidate = 0; candidate < 360; ++candidate) {
        if (context.batch_cost[candidate] < minimum) minimum = context.batch_cost[candidate];
        if (context.batch_cost[candidate] > maximum) maximum = context.batch_cost[candidate];
        sum += context.batch_cost[candidate];
    }
    double seconds = (double)(monotonic_ns() - begin) / 1e9;
    printf("CMLLEO_REVERSE_BATCH_SAMPLE|states=360|reverse_cost=%u|min=%u|max=%u|average=%.6f|nodes=%" PRIu64 "|seconds=%.6f\n",
           reverse_cost, minimum, maximum, (double)sum / 360.0, context.nodes, seconds);
    free(context.reverse); free(context.transpositions); free(context.lse); free(context.eo_center); free(context.block_edge); free(context.corner);
}

int main(int argc, char **argv) {
    if (argc == 4 && strcmp(argv[1], "--reverse-wave") == 0) {
        validate_moves();
        reverse_entry *table = build_reverse_wave(argv[2], (unsigned)strtoul(argv[3], NULL, 10));
        free(table);
        return 0;
    }
    if (argc == 3 && strcmp(argv[1], "--build-corner-pdb") == 0) {
        validate_moves();
        build_corner_pdb(argv[2]);
        return 0;
    }
    if (argc == 3 && strcmp(argv[1], "--build-block-edge-pdb") == 0) {
        validate_moves();
        build_block_edge_pdb(argv[2]);
        return 0;
    }
    if (argc == 3 && strcmp(argv[1], "--build-eo-center-pdb") == 0) {
        validate_moves();
        build_eo_center_pdb(argv[2]);
        return 0;
    }
    if (argc == 4 && strcmp(argv[1], "--solve-sample") == 0) {
        validate_moves();
        solve_sample(argv[2], argv[3]);
        return 0;
    }
    if (argc == 4 && strcmp(argv[1], "--solve-batch-sample") == 0) {
        validate_moves();
        solve_batch_sample(argv[2], argv[3]);
        return 0;
    }
    if (argc == 5 && strcmp(argv[1], "--solve-reverse-batch-sample") == 0) {
        validate_moves();
        solve_reverse_batch_sample(argv[2], argv[3], (unsigned)strtoul(argv[4], NULL, 10));
        return 0;
    }
    uint64_t iterations = UINT64_C(10000000);
    if (argc == 2) iterations = strtoull(argv[1], NULL, 10);
    validate_moves();

    cube_state state;
    memset(&state, 0, sizeof(state));
    for (unsigned index = 0; index < 12; ++index) state.edge_piece[index] = (uint8_t)index;
    for (unsigned index = 0; index < 8; ++index) state.corner_piece[index] = (uint8_t)index;
    for (unsigned index = 0; index < 6; ++index) state.center_piece[index] = (uint8_t)index;
    uint64_t begin = monotonic_ns();
    for (uint64_t iteration = 0; iteration < iterations; ++iteration) {
        apply_move(&state, (unsigned)((iteration * 7 + 3) % CMLLEO_MOVE_COUNT));
    }
    uint64_t elapsed = monotonic_ns() - begin;
    uint64_t checksum = 0;
    const uint8_t *bytes = (const uint8_t *)&state;
    for (unsigned index = 0; index < sizeof(state); ++index) checksum = checksum * 131 + bytes[index];
    double seconds = (double)elapsed / 1e9;
    printf("CMLLEO_KERNEL|moves=%" PRIu64 "|seconds=%.6f|million_moves_per_second=%.3f|checksum=%" PRIu64 "\n",
           iterations, seconds, (double)iterations / seconds / 1e6, checksum);
    return 0;
}
