__author__ = 'baio'

import json

data_file = "./parse_train.dat"
data_file_rare = "./parse_train.rare.dat"

is_array = lambda var: isinstance(var, (list, tuple))

def append_one(dict, key):
    if key in dict:
        dict[key] += 1
    else:
        dict[key] = 1


words = dict()

def iter_list(list, is_replace):
    for item in list:

        if not is_array(item):
            continue

        if len(item) == 2:
            word = item[1]
            if not is_replace:
                append_one(words, word)
            else:
                if words[word] < 5:
                    item[1] = "_RARE_"
        else:
            iter_list(item, is_replace)

def readJson(file_name):
    for df in open(file_name):
        yield json.loads(df)

if __name__ == "__main__":
    j = list(readJson(data_file))
    iter_list(j, False)
    iter_list(j, True)

    #print j

    with open(data_file_rare, 'w') as wf:
        for i in j:
            json.dump(i, wf)
            wf.write("\n")





