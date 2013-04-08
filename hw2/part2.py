__author__ = 'baio'

import sys

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
    count_X = G["N"][X]
    return count_X_w / float(count_X)

def q_X_YZ(X, Y, Z):
    count_X_w = G["R"].get((X, Y, Z), 0)
    count_X = G["N"][X]
    return count_X_w / float(count_X)

def parse_sent(sent):


    #initialization
    words = sent.split()
    n = len(words)
    N = G["N"]
    R = G["R"]

    pi = [[dict(zip(N.keys(), [0] * len(N))) for j in xrange(n)] for i in xrange(n)]

    for i in xrange(n):
        for X in N:
            pi[i][i][X] = q_X_w(X, words[i])

    for l in range(1, n):
        for i in range(1, n - l):
            j = i + l
            for X in N:
                for rule in [t for t in R.keys() if t[0] == X and len(t) == 3]:
                    Y = rule[1]
                    Z = rule[2]
                    for s in range(i, j):
                        pi[i][j][X] = q_X_YZ(X, Y, Z) * pi[i][s][Y] * pi[s + 1][j][Z]

    print pi

init("data/parse_train.counts.out")

parse_sent("What was the monetary value of the Nobel Peace Prize in 1989 ?")
"""
for sent in open("data/parse_dev.dat"):
    break
"""
