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
        n_e = filter(lambda x: x[0] == e_w, count_e_f)
        for e_f_w in n_e:
            f_w = e_f_w[1]
            if e_w not in t_e_f:
                t_e_f[e_w] = dict()
            t_e_f[e_w][f_w] = 1 / float(len(n_e))
    """initialization / end"""

    def t_f_given_e(e_w, f_w):
        return t_e_f[e_w][f_w] / float(t_e[e_w])

    for t in xrange(iter_count):
        """iter iter_count times, to converge to result"""
        count_e.clear()
        count_e_f.clear()
        for k in xrange(len(e)):
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
                    sigma = t_f_given_e(e_w, f_w) / sum(map(lambda x: t_f_given_e(x, f_w), e_s))
                    """update counts"""
                    count_e_f[(e_w, f_w)] += sigma
                    count_e[e_w] += sigma
        """After each iteration through the parallel corpus, we revise our estimate for t parameters"""
        for e_w in count_e:
            t_e[e_w] = count_e[e_w]
            for e_f_w in filter(lambda x: x[0] == e_w, count_e_f):
                t_e_f[e_w][e_f_w[1]] = count_e_f[(e_w, e_f_w[1])]  / count_e[e_w]

        """
        Calculate alignments max probabilities for each word in each sentence.
        a(i) = arg max{j = 0..1} t(f(i)|e(j))
        """
        res = []
        for k in xrange(len(e)):
            e_s = e[k].split(' ')
            f_s = f[k].split(' ')
            """sentence align"""
            s_a = []
            res.append(s_a)
            for f_w in f_s:
                max_t = 0
                max_arg = 0
                for e_i in xrange(len(e_s)):
                    e_w = e_s[e_i]
                    t = t_e_f[e_w][f_w]
                    if max_t < t:
                        max_t = t
                        max_arg = e_i
                s_a.append(max_arg + 1)
    return res
