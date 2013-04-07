
###First step###
By default, hadoop divides sentences by new line, so format which given (new line divides separate words and empty string divide sentenes) doesn't fit here.
Let's reformat given train file to known for hadoop.

`Reformat.exe` - reformat example train data (new line divides separate words and empty string divide sentenes) to hadoop default faormat (new line divides separate sentences and `\t` divides separate words)
`Reformat.exe example.train example.hadoop.train`

[hadoop on azure (C#)](//http://www.windowsazure.com/en-us/develop/net/tutorials/hadoop-and-data/#segment1)

###Second step###
Map words count
Map.WordsCount

###Third step###
Map n-grams
Map.NGrams
