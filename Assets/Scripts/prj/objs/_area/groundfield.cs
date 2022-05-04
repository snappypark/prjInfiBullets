using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundfield : MonoBehaviour
{
    [SerializeField] Renderer _renderGround;
    [SerializeField] Renderer _renderleft;
    [SerializeField] Renderer _renderRight;
    
#if UNITY_EDITOR
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.C))
        {
            Color col1 = _renderGround.sharedMaterial.GetColor("_Color");
            Color col2 = _renderleft.sharedMaterial.GetColor("_Color");
            Debug.Log(
                string.Format("new Color({0:0.0000}f, {1:0.0000}f, {2:0.0000}f), new Color({3:0.0000}f, {4:0.0000}f, {5:0.0000}f) ", 
                col1.r, col1.g, col1.b, col2.r, col2.g, col2.b) );
        }
    }
#else
#endif


    public void SetColor(int stageIdx)
    {
        switch(stageIdx)
        {
            case 0: case 1: case 2:
            SetColor(new Color(0.6176f, 0.7075f, 0.6041f), new Color(0.4993f, 0.6415f, 0.6205f)  ); 
            break;
            case 3: case 4: case 5:
            SetColor(new Color(0.4116f, 0.5052f, 0.6981f), new Color(0.3269f, 0.4075f, 0.6132f)  );
            break;
            case 6: case 7: case 8: case 9:
            SetColor(new Color(0.7925f, 0.5943f, 0.6182f), new Color(0.6981f, 0.4380f, 0.4596f)  ); 
            break;
            default:
            int tmp = stageIdx/5;
            switch(tmp%13)
            { 
                case 0: SetColor(new Color(0.7047f, 0.7264f, 0.5311f), new Color(0.6072f, 0.6132f, 0.4309f) ); break; // yelo
                case 1: SetColor(new Color(0.5370f, 0.6812f, 0.7641f), new Color(0.3838f, 0.5861f, 0.7075f) ); break; // sky

                case 2: SetColor(new Color(0.5333f, 0.7490f, 0.6392f), new Color(0.4004f, 0.6792f, 0.5763f) ); break; //
                case 3: SetColor(new Color(0.7756f, 0.7003f, 0.8113f), new Color(0.7736f, 0.6021f, 0.6863f) ); break; 
                case 4: SetColor(new Color(0.4862f, 0.6739f, 0.8962f), new Color(0.3775f, 0.5560f, 0.7925f) ); break;  // b
                case 5: SetColor(new Color(0.4862f, 0.6739f, 0.8962f), new Color(0.3775f, 0.5560f, 0.7925f) ); break;  

                case 6: SetColor(new Color(0.6981f, 0.6479f, 0.5696f), new Color(0.6886f, 0.5701f, 0.4580f) ); break;  
                case 7: SetColor(new Color(0.5036f, 0.7264f, 0.6304f), new Color(0.3706f, 0.6603f, 0.5816f) ); break;  //g
 
                case 8: SetColor(new Color(0.6109f, 0.6664f, 0.9057f), new Color(0.4576f, 0.5677f, 0.8585f) ); break; 
                case 9: SetColor(new Color(0.8113f, 0.7003f, 0.7922f), new Color(0.7264f, 0.5722f, 0.7194f) ); break; 

                case 10: SetColor(new Color(0.6176f, 0.7075f, 0.6041f), new Color(0.4993f, 0.6415f, 0.6205f)  );  break; 
                case 11: SetColor(new Color(0.4116f, 0.5052f, 0.6981f), new Color(0.3269f, 0.4075f, 0.6132f)  ); break; 
                case 12: SetColor(new Color(0.7925f, 0.5943f, 0.6182f), new Color(0.6981f, 0.4380f, 0.4596f)  );   break; 

            }
            break;
        }
    }

    
    public void SetColor(Color col1,Color col2)
    {
        _renderGround.sharedMaterial.SetColor("_Color", col1);
        _renderleft.sharedMaterial.SetColor("_Color", col2);
    }
}
