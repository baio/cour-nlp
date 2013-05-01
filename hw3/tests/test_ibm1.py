# -*- coding: utf-8 -*-
__author__ = 'baio'

import unittest
from hw3.ibm1.estimator import align_lines

class TestIbm1(unittest.TestCase):


    def test_initialize_debug(self):
        res = align_lines(["the dog", "the cat", "the rat", "some thing"],
                         ["athe adog", "bthe bcat", "athe crat", "asome athing"])
        self.assertEquals(res["dog"]["adog"], 1.0)

if __name__ == '__main__':
    unittest.main()

