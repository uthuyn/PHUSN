using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    class UtilityChainList
    {
        IList<int> seqIDList;

        public IList<int> SeqIDList
        {
            get { return seqIDList; }
            set { seqIDList = value; }
        }
        IList<UtilityChain> listOfUtilityChain;

        public IList<UtilityChain> ListOfUtilityChain
        {
            get { return listOfUtilityChain; }
            set { listOfUtilityChain = value; }
        }
       public UtilityChainList() { }
       public UtilityChainList(IList<int> seqIDList, IList<UtilityChain> listOfUtilityChain)
        {
            this.seqIDList = seqIDList;
            this.listOfUtilityChain = listOfUtilityChain;
        }
    }
