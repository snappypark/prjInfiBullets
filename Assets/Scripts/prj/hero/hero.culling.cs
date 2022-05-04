using UnityEngine;

public partial class hero
{
    public static int backZ;
    public static culling_ culling = new culling_();
    
    public class culling_
    {
        int _preZ=-1000, _curZ=-1000;
        int backSize = 6, size = 36;

        public void Refresh()
        {
            //_preZ = _curZ = -1000;
            OnUpdate(-2000);
        }

        public void OnUpdate(int ptZ)
        {
            if (_curZ == ptZ)
                return;
            _preZ = _curZ;  _curZ = ptZ - backSize;
            int gap = Mathf.Clamp(_curZ - _preZ, -size, size);
            int zn;
            // 
            
            if(gap > 0)
            {
                
                for(int i=0; i<gap; ++i)
                {
                    int z = _preZ +i;
                    for(int x=1; x<cells.MaxX; ++x)
                        if(x > 0 && x < cells.MaxX && z > 0 && z < cells.MaxZ)
                            objs.InactiveObj(cells.Get(x, z));
                }

                zn = 1 + Mathf.Max(_preZ+size, _curZ);
                for(int i=0; i<gap; ++i)
                {
                    int z = zn +i;
                    for(int x=1; x<cells.MaxX; ++x)
                        if(x > 0 && x < cells.MaxX && z > 0 && z < cells.MaxZ)
                            objs.ActiveObj(cells.Get(x, z));
                }
            }
            else
            {
                
                zn = 1 + Mathf.Max(_preZ, _curZ+size);
                for(int i=0; i>gap; --i)
                {
                    int z = zn - i;
                    for(int x=1; x<cells.MaxX; ++x)
                        if(x > 0 && x < cells.MaxX && z > 0 && z < cells.MaxZ)
                            objs.InactiveObj(cells.Get(x, z));
                }

                for(int i=0; i>gap; --i)
                {
                    int z = _curZ - i;
                    for(int x=1; x<cells.MaxX; ++x)
                        if(x > 0 && x < cells.MaxX && z > 0 && z < cells.MaxZ)
                            objs.ActiveObj(cells.Get(x, z));
                }

            }
        }
    }
}
