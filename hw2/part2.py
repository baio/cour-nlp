__author__ = 'baio'

import sys
import copy
import operator
import json
from hw2 import pretty_print_tree

G = None

def simple_counts_iterator(f):
    """
    (type, count, rules[])
    """
    l = f.readline()
    while l:
        line = l.strip()
        if line:
            fields = line.split(" ")
            cnt = int(fields[0])
            type = fields[1]
            rule = fields[2:]
            yield type, cnt, rule
        l = f.readline()


def init(counts_file_name):

    try:
        input = file(counts_file_name,"r")
    except IOError:
        sys.stderr.write("ERROR: Cannot read inputfile %s.\n" % arg)
        sys.exit(1)

    counts = list(simple_counts_iterator(input))
    global G
    G = dict(
        N=dict([(t[2][0], t[1]) for t in counts if t[0] == "NONTERMINAL"]),
        E=dict(set((t[2][1], t[1]) for t in counts if t[0] == "UNARYRULE")),
        R=dict([(tuple(t[2]), t[1]) for t in counts if t[0] == "UNARYRULE"] + [(tuple(t[2]), t[1]) for t in counts if t[0] == "BINARYRULE"])
    )

def q_X_w(X, w):
    count_X_w = G["R"].get((X, w), 0)
    if count_X_w == 0 and len(filter(lambda x: x[1] == w, G["R"].keys())) == 0:
        count_X_w = G["R"].get((X, "_RARE_"), 0)
    count_X = G["N"][X]
    return count_X_w / float(count_X)

def q_X_YZ(X, Y, Z):
    count_X_w = G["R"].get((X, Y, Z), 0)
    count_X = G["N"][X]
    return count_X_w / float(count_X)

def print_pi(pi, i, j):
    for k, v in pi[i][j].iteritems():
        if v != 0:
            print "pi({},{},{})={:e}".format(i, j, k, v)
    print "\n"

def print_bp(pi, i, j):
    for k, v in pi[i][j].iteritems():
        if v != 0:
            print "bp({},{},{})={}, {}".format(i, j, k, v[0], v[1])
    print "\n"

def iter_bp(bp, sent, start, end, bp_iter):
    s = bp_iter[1]
    tag_left = bp_iter[0][1]
    tag_right = bp_iter[0][2]
    bp_left = bp[start][s][tag_left]
    bp_right = bp[s + 1][end][tag_right]

    if s == start:
        ret_1 = [tag_left, sent[start]]
    else:
        ret = iter_bp(bp, sent, start, s, bp_left)
        ret_1 = [tag_left, ret[0], ret[1]]
    if s + 1 == end:
        ret_2 = [tag_right, sent[end]]
    else:
        ret = iter_bp(bp, sent, s + 1, end, bp_right)
        ret_2 = [tag_right, ret[0], ret[1]]
    return [ret_1, ret_2]

def parse_sent(sent):

    #initialization
    words = sent.split()
    n = len(words)
    N = G["N"]
    R = G["R"]

    pi = [[dict(zip(N.keys(), [0] * len(N))) for j in xrange(n)] for i in xrange(n)]
    bp = copy.deepcopy(pi)

    for i in xrange(n):
        for X in N:
            pi[i][i][X] = q_X_w(X, words[i])


    """
    print_pi(pi, 0, 0)
    print_pi(pi, 1, 1)
    print_pi(pi, 2, 2)
    """


    for l in range(1, n):
        for i in range(0, n - l):
            j = i + l
            for X in N:
                for rule in [t for t in R.keys() if t[0] == X and len(t) == 3]:
                    Y = rule[1]
                    Z = rule[2]
                    for s in range(i, j):
                        pi_max = q_X_YZ(X, Y, Z) * pi[i][s][Y] * pi[s + 1][j][Z]
                        if pi_max > pi[i][j][X]:
                            pi[i][j][X] = pi_max
                            bp[i][j][X] = (rule, s)
            #print_pi(pi, i, j)
            #print_bp(bp, i, j)

    #move along with back pointers
    pi_max = max(filter(lambda (key, val): key  == "SBARQ", pi[0][n - 1].iteritems()), key=operator.itemgetter(1))
    bp_max = bp[0][n - 1][pi_max[0]]
    #print pi_max
    #print bp_max
    tree = iter_bp(bp, words, 0, n - 1, bp_max)
    return [pi_max[0], tree[0], tree[1]]


init("data/cfg_vert.rare.counts")

#parse_sent("What was the monetary value of the Nobel Peace Prize in 1989 ?")
#parse_sent("How many miles is it from London , England to Plymouth , England ?")
#tree = parse_sent("What are geckos ?")
#pretty_print_tree.pretty_print_tree(json.dumps(tree))

with open("data/parse_dev.p3.out", "w") as f:
    for sent in open("data/parse_dev.dat"):
        tree = parse_sent(sent)
        json.dump(tree, f)
        f.write("\n")




