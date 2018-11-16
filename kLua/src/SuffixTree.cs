using System;
using System.Collections.Generic;
namespace kLua.src
{
    public class SuffixTree{
        RootNode root = new RootNode();

        static public void Main(string[] args){
            List<string> examples = new List<string>{ "blah", "ostrich", "hernodale" };
            var st = new SuffixTree(examples);
            List<string> hits = st.Find("la");
            foreach(var hit in hits){
                Console.WriteLine(hit);
            }
        }

        public SuffixTree(List<string> strings)
        {
            foreach(var str in strings){
                // adds a "sentinel" char at the beginning '_'
                // for simplicity in the algorithm.
                root.addString('_' + str,str);
            }
        }

        List<string> Find(string str){
            List<string> matchingStrings = new List<string>();
            // '_' is a sentinel char. It will be gobbled up to no effect.
            root.AddMatchingStrings('_'+str,matchingStrings);
            return matchingStrings;
        }

        class RootNode:SuffixTreeNode{
            public RootNode():base(null){
                this.root = this;
            }
        }

        class SuffixTreeNode : Dictionary<char, SuffixTreeNode>
        {
            public RootNode root;
            public SuffixTreeNode(RootNode root){
                this.root = root;
            }

            List<string> matchingStrings = new List<string>();
            char ch;
            public void addString(string rest,string fullstring){
                ch = rest[0];
                if(rest.Length>1){
                    SuffixTreeNode newNode;
                    if(!root.TryGetValue(rest[1], out newNode))
                    {
                        newNode = new SuffixTreeNode(root);
                        root.Add(rest[1], newNode);
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
                    SuffixTreeNode nextNode;
                    if(TryGetValue(str[1],out nextNode)){
                        AddMatchingStrings(str.Substring(1), matchingStrings);
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
