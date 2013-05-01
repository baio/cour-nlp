# -*- coding: utf-8 -*-
__author__ = 'baio'
from collections import defaultdict

def initialize(e, f):
    """
    getting sparse map - [e : dict(f , t(f|e))]
    t(f|e) = c(e, f) / c(e)
    """
    count_e = dict()
    count_e_f = dict()
    for i in xrange(len(e)):
        for e_w in e[i].split(' '):
            c_e = count_e.get(e_w, 0)
            count_e[e_w] = c_e + 1
            for f_w in f[i].split(' '):
                c_e_f = count_e_f.get((e_w, f_w), 0)
                count_e_f[(e_w, f_w)] = c_e_f + 1
    res = dict()
    for e_w in count_e:
        e_map = dict()
        res[e_w] = e_map
        for e_f_w in filter(lambda x: x[0] == e_w, count_e_f):
            e_map[e_f_w[1]] = count_e_f[e_f_w]  / float(count_e[e_w])
    return res


def align_lines(e, f, iter_count=5):
    """
    getting sparse map - [e : dict(f , t(f|e))]
    t(f|e) = c(e, f) / c(e)
    """
    t_e_f = dict()
    t_e = dict()
    count_e = defaultdict(int)
    count_e_f = defaultdict(int)

    """initialization"""
    for i in xrange(len(e)):
        for e_w in e[i].split(' '):
            count_e[e_w] += 1
            for f_w in f[i].split(' '):
                count_e_f[(e_w, f_w)] += 1
    for e_w in count_e:
        t_e[e_w] = count_e[e_w]
        for e_f_w in filter(lambda x: x[0] == e_w, count_e_f):
            t_e_f[(e_w, f_w)] = count_e_f[e_f_w]  / count_e[e_w]
    """initialization / end"""

    def t_f_given_e(e_w, f_w):
        t_e_f[(e_w, f_w)] / float(t_e[e_w])

    for t in xrange(iter_count):
        count_e.clear()
        count_e_f.clear()
        """iter iter_count times, to converge to result"""
        for k in xrange(e):
            """iter through lines (sentences)"""
            """english sentence"""
            e_s = e[k].split(' ')
            """foreign sentence"""
            f_s = f[k].split(' ')
            """english sentence length"""
            l = len(e_s)
            """foreigh sentence length"""
            m = len(f_s)
            """iter through words in foreign sentence"""
            for i in xrange(m):
                """foreign word"""
                f_w = f_s[i]
                """iter through words in english sentence"""
                for j in xrange(l):
                    """english word"""
                    e_w = e_s[j]
                    """calculate sigma"""
                    sigma = t_f_given_e(e_w, f_w) / sum(lambda x: t_f_given_e(x, f_w), e_s)
                    """update counts"""
                    count_e_f[(e_w, f_w)] += sigma
                    count_e[e_w] += sigma
        """After each iteration through the parallel corpus, we revise our estimate for t parameters"""
        for e_w in count_e:
            t_e[e_w] = count_e[e_w]
            for e_f_w in filter(lambda x: x[0] == e_w, count_e_f):
                t_e_f[e_w][e_f_w[1]] = count_e_f[e_f_w]  / count_e[e_w]
    return t_e_f
