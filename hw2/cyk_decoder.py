__author__ = 'baio'

import sys

G = None

def simple_counts_iterator(f):
    """
    (type, count, (tag) | ((X), (Y | [Y1,Y2])))
    """
    l = f.readline()
    while l:
        line = l.strip()
        if line: # Nonempty line
            # Extract information from line.
            # Each line has the format
            # word pos_tag phrase_tag ne_tag
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
        N=[t[2][0] for t in counts if t[0] == "NONTERMINAL"],
        E=set(t[2][1] for t in counts if t[0] == "UNARYRULE"),
        R=[t[2] for t in counts if t[0] == "BINARYRULE"] + [t[2] for t in counts if t[0] == "UNARYRULE"]
    )


def parse_sent(sent):

    n = len(sent)

    #initialization

    l = len(G["N"])

    pi = [n, n, l]

    for X in G["N"]:
        for i in sequency(0, n + 1):
            word = sent[i]
            if (word, X) in nonterminals_count:
                pi[i, i, X] = nonterminals_count[word, X] / words_count[word]
            else:
                pi[i, i, X] = 0

init("parse_train.counts.out")

for sent in open("parse_dev.dat"):
    parse_sent(sent)
