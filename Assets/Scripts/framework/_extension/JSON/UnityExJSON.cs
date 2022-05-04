using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngineExJSON
{
    public static class NewJSONObj
    {
        public static JSONObject With(
            string name, JSONObject parent, JSONObject.Type type)
        {
            JSONObject jsobj = new JSONObject(type);
            parent.AddField(name, jsobj);
            return jsobj;
        }

        public static JSONObject With(
            JSONObject parent, JSONObject.Type type)
        {
            JSONObject jsobj = new JSONObject(type);
            parent.Add(jsobj);
            return jsobj;
        }


        public static JSONObject AddObj(this JSONObject parent, JSONObject.Type type)
        {
            JSONObject jsobj = new JSONObject(type);
            parent.Add(jsobj);
            return jsobj;
        }
    }

    public struct jsArr
    {
        JSONObject _arr;
        public int num;
        public int i;
        public jsArr(JSONObject arr)
        {
            i = 0;
            _arr = arr;
            num = arr.Count;
        }

        public void SetIdx(int idx)
        {
            i = idx;
        }

        public JSONObject Obj
        {
            get
            {
                return _arr[i++];
            }
        }

        public float F
        {
            get
            {
                return _arr[i++].f;
            }
        }

        public int Int
        {
            get
            {
                return (int)_arr[i++].i;
            }
        }

        public short Short
        {
            get
            {
                return (short)_arr[i++].i;
            }
        }

        public long Long
        {
            get
            {
                return _arr[i++].i;
            }
        }

        public byte Byte
        {
            get
            {
                return (byte)_arr[i++].i;
            }
        }

        public Vector3 Vec3_Of2
        {
            get
            {
                return new Vector3(_arr[i++].f, _arr[i++].f);
            }
        }
        /*
        public edges.eType EdgeType
        {
            get
            {
                return (edges.eType)_arr[i++].i;
            }
        }

        public dots.eType DotType
        {
            get
            {
                return (dots.eType)_arr[i++].i;
            }
        }*/

        /*
    public halfEdge.eType HalfType
    {
        get
        {
            return (halfEdge.eType)_arr[i++].i;
        }
    }

    public portals.eType PortalType
    {
        get
        {
            return (portals.eType)_arr[i++].i;
        }
    }

    public rDot.eState rDotType
    {
        get
        {
            return (rDot.eState)_arr[i++].i;
        }
    }
    */
    }
}
