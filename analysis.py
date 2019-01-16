import numpy as np
import matplotlib.pyplot as pp

pp.plot(np.linspace(1, 32*300, 32*300) / 32, [float(x) for x in open('beats.txt').read().split()])
pp.show()