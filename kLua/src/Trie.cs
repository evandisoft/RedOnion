using System;
using System.Collections.Generic;
namespace kLua.src
{
    public class Trie{
        RootNode root = new RootNode();
        // Other than making the algorithm simpler, the 'sentinel' is ignored
        const char sentinel = '_';

        public Trie(List<string> strings)
        {
            foreach(var str in strings){
                root.addString(sentinel + str,str);
            }
        }

        public List<string> Find(string str){
            List<string> matchingStrings = new List<string>();
            root.AddMatchingStrings(sentinel+str,matchingStrings);
            return matchingStrings;
        }

        class RootNode:TrieNode{}

        class TrieNode : Dictionary<char, TrieNode>
        {

            List<string> matchingStrings = new List<string>();
            char ch;
            public void addString(string rest,string fullstring){
                ch = rest[0];
                if(rest.Length>1){
                    TrieNode newNode;
                    if(!TryGetValue(rest[1], out newNode))
                    {
                        newNode = new TrieNode();
                        Add(rest[1], newNode);
                    }
                    newNode.addString(rest.Substring(1), fullstring);
                }
                else{
                    matchingStrings.Add(fullstring);
                }
            }

            public void AddMatchingStrings(string str,List<string> matchingStrings){
                if(str.Length>1){
                    TrieNode nextNode;
                    if(TryGetValue(str[1],out nextNode)){
                        nextNode.AddMatchingStrings(str.Substring(1), matchingStrings);
                    }else{
                        return;
                    }
                }else{
                    AddAllStrings(matchingStrings);
                }
            }

            public void AddAllStrings(List<string> matchingStrings){
                matchingStrings.AddRange(this.matchingStrings);
                foreach(var item in this){
                    item.Value.AddAllStrings(matchingStrings);
                }
            }
        }
    }
}
