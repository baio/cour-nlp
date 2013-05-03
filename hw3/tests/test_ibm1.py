# -*- coding: utf-8 -*-
__author__ = 'baio'

import unittest
from hw3.ibm1.estimator import align_lines

class TestIbm1(unittest.TestCase):


    def test_initialize_debug(self):
        return
        t_e_f = align_lines(["the dog", "the cat", "the rat", "some thing"],
                         ["athe adog", "bthe bcat", "athe crat", "asome athing"])

        with open("data/t_e_f.out", "w") as fs:
            for e in t_e_f:
                for f_t in t_e_f[e]:
                    fs.write("{} {} {}\n".format( e, f_t, t_e_f[e][f_t]))

    def test_align_corpus(self):
        #return
        with open("data/corpus.en") as fs:
            e = fs.read().split("\n")
        with open("data/corpus.es") as fs:
            f = fs.read().split("\n")
        t_e_f = align_lines(e, f)

        with open("data/trained_align.out", "w") as fs:
            for e in t_e_f:
                for f_t in t_e_f[e]:
                    fs.write("{} {} {}\n".format( e, f_t, t_e_f[e][f_t]))

        """
        with open("data/dev.out", "w") as fs:
            for i_al in xrange(len(als)):
                al = als[i_al]
                for i_w in xrange(len(al)):
                    fs.write("{} {} {}\n".format( i_al + 1, al[i_w], i_w + 1))
        """

if __name__ == '__main__':
    unittest.main()

