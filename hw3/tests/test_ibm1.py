# -*- coding: utf-8 -*-
__author__ = 'baio'

import unittest
from hw3.ibm1.estimator import align_lines

class TestIbm1(unittest.TestCase):


    def test_initialize_debug(self):
        return
        als = align_lines(["the dog", "the cat", "the rat", "some thing"],
                         ["athe adog", "bthe bcat", "athe crat", "asome athing"])
        with open("data/dev.out", "w") as fs:
            for i_al in xrange(len(als)):
                al = als[i_al]
                for i_w in xrange(len(al)):
                    fs.write("{} {} {}\n".format( i_al + 1, al[i_w], i_w + 1))

    def test_dev_out(self):
        with open("data/dev.en") as fs:
            e = fs.read().split("\n")
        with open("data/dev.es") as fs:
            f = fs.read().split("\n")
        als = align_lines(e, f)
        with open("data/dev.out", "w") as fs:
            for i_al in xrange(len(als)):
                al = als[i_al]
                for i_w in xrange(len(al)):
                    fs.write("{} {} {}\n".format( i_al + 1, al[i_w], i_w + 1))

if __name__ == '__main__':
    unittest.main()

