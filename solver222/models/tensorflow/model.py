import os
import numpy as np
from numpy import genfromtxt
from keras.models import Sequential
from keras.layers.core import Dense
from keras.callbacks import ModelCheckpoint

path = os.path.dirname(os.path.realpath(__file__))

data = genfromtxt(path + "/data.csv", delimiter=',')
labels = genfromtxt(path + "/labels.csv", delimiter=',')

model = Sequential()
model.add(Dense(400, input_dim=16, activation='relu'))
model.add(Dense(400, activation='sigmoid'))
model.add(Dense(12, activation='softmax'))

model.compile(loss='mean_squared_error',
              optimizer='adam',
              metrics=['accuracy'])
checkpoint = ModelCheckpoint(path + "/model.hdf5", monitor='val_loss', verbose=0, save_best_only=False, save_weights_only=False, mode='auto', period=1)
model.fit(data, labels, nb_epoch=1000000, verbose=2, callbacks=[checkpoint])

print(model.predict(data).round())