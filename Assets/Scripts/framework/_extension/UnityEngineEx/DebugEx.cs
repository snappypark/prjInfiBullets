using UnityEngine;

namespace UnityEngineEx
{
    public static class DebugEx
    {
        
        public static void DrawLineOnXZ(float x1, float z1, float x2, float z2, Color color, float duration = 1)
        {
            Debug.DrawLine(new Vector3(x1, 0, z1), new Vector3(x2, 0, z2), color, duration);
        }

        public static void DrawLineOnXY(this Transform tr, float dx, float dy, Color color, float duration = 1)
        {
            Debug.DrawLine(tr.localPosition, new Vector3(tr.localPosition.x + dx, tr.localPosition.y + dy), color, duration);
        }

        public static void DrawSqureXZ(Vector3 pos, float radius, Color color, float duration = 1)
        {
            Debug.DrawLine(new Vector3(pos.x-radius, 0, pos.z-radius), new Vector3(pos.x-radius, 0, pos.z+radius), color, duration);
            Debug.DrawLine(new Vector3(pos.x-radius, 0, pos.z-radius), new Vector3(pos.x+radius, 0, pos.z-radius), color, duration);
            Debug.DrawLine(new Vector3(pos.x+radius, 0, pos.z+radius), new Vector3(pos.x-radius, 0, pos.z+radius), color, duration);
            Debug.DrawLine(new Vector3(pos.x+radius, 0, pos.z+radius), new Vector3(pos.x+radius, 0, pos.z-radius), color, duration);
        }
        
        public static void DrawSqureXZ(Vector3 pos1, Vector3 pos2, float radius, Color color, float duration = 1)
        {
            Debug.DrawLine(new Vector3(pos1.x-radius, 0, pos1.z-radius), new Vector3(pos1.x-radius, 0, pos2.z+radius), color, duration);
            Debug.DrawLine(new Vector3(pos1.x-radius, 0, pos1.z-radius), new Vector3(pos2.x+radius, 0, pos1.z-radius), color, duration);
            Debug.DrawLine(new Vector3(pos2.x+radius, 0, pos2.z+radius), new Vector3(pos1.x-radius, 0, pos2.z+radius), color, duration);
            Debug.DrawLine(new Vector3(pos2.x+radius, 0, pos2.z+radius), new Vector3(pos2.x+radius, 0, pos1.z-radius), color, duration);
        }


        public static void DrawCircleXZ(float posx, float posy, float posz, float radius, Color col)
        {
            float xradius = radius;
            float zradius = radius;
            int segments = 16;
            float x = 0f;
            float xo = 0f;
            float z = 0f;
            float zo = 0f;
            float angle = 0f;
            
            Vector3 newCenter = new Vector3(posx, posy, posz);
        
            var vo = Vector3.zero;
            var vn = Vector3.zero;
        
            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * zradius;
            
                vo = new Vector3(newCenter.x + xo, posy, newCenter.z + zo);
                vn = new Vector3(newCenter.x + x,  posy, newCenter.z + z);
                
                if (i > 0) Debug.DrawLine(vo, vn, col, 2);
            
                xo = x;
                zo = z;
                angle += (360f / segments);
            }
        }

        public static void DrawCircle(Vector3 pos, float radius, Color col)
        {
            float xradius = radius;
            float zradius = radius;
            int segments = 16;
            float x = 0f;
            float xo = 0f;
            float y = 0f;
            float yo = 0f;
            float angle = 0f;
            
            Vector3 newCenter = pos;
        
            var vo = Vector3.zero;
            var vn = Vector3.zero;
        
            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                y = Mathf.Cos(Mathf.Deg2Rad * angle) * zradius;
            
                vo.x = newCenter.x + xo;
                vo.y = newCenter.y + yo;
            
                vn.x = newCenter.x + x;
                vn.y = newCenter.y + y;
                
                if (i > 0) Debug.DrawLine(vo, vn, col, 2);
            
                xo = x;
                yo = y;
                angle += (360f / segments);
            }
        }
    }
}
