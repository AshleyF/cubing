#!/bin/sh
set -eu

if [ "$#" -ne 1 ]; then
  echo "usage: $0 WORK_DIRECTORY" >&2
  exit 2
fi

tools_directory=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
repository=$(CDPATH= cd -- "$tools_directory/../.." && pwd)
work_directory=$1
mkdir -p "$work_directory"

dotnet fsi "$tools_directory/GenerateCmllEoMoveData.fsx"
cc -O3 -std=c11 -Wall -Wextra -pedantic "$tools_directory/CmllEoKernel.c" -o "$work_directory/cmll-eo-kernel"
"$work_directory/cmll-eo-kernel" --build-corner-pdb "$work_directory/rouxlab-cmll-eo-corners-v1.pdb"
"$work_directory/cmll-eo-kernel" --build-block-edge-pdb "$work_directory/rouxlab-cmll-eo-block-edges-v1.pdb"
"$work_directory/cmll-eo-kernel" --build-eo-center-pdb "$work_directory/rouxlab-cmll-eo-eo-center-v1.pdb"
"$work_directory/cmll-eo-kernel" --solve-sample "$work_directory" "$repository/solver333/Data/lse-policy-v1.dat"
