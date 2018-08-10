using UnityEngine;

namespace DefaultNamespace
{

    public class ResultType
    {
        public GameObject Obj { get; private set; }
        public int From { get; private set; }
        public int To { get; private set; }

        public ResultType(GameObject _obj, int _from, int _to)
        {
            this.Obj = _obj;
            this.From = _from;
            this.To = _to;
        }

        public override string ToString()
        {
            return Obj.name + ", " + this.From + " -> " + this.To;
        }
    }
}