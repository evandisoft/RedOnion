grammarName=`basename $1 .g4`
java -jar antlr-4.7.1-complete.jar -package Grammar."$grammarName"Parsing -visitor -Dlanguage=CSharp $1