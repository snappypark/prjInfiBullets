using UnityEngine;
using UnityEngineEx;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(stageEditor))]
public class stageEditorEditor : Editor
{
    public override bool RequiresConstantRepaint() { return true; }
    stageEditor _ui;
    enum objCategory:byte{walls, ballrs, traps, etc, balls,}
    objCategory _objCategory = objCategory.walls; 
    byte[] _objtypes = new byte[5]{0,0,0,0,0};

    
    public override void OnInspectorGUI()
    {
        if(stageEditor.setAlpha0)
        {
            //_objCategory = objCategory.walls;
            _objCategory = objCategory.etc;
            _objtypes[3] = (int)objEtc.Path;
            stageEditor.setAlpha0 = false;
        }
        else if(stageEditor.setAlpha1)
        {
            //_objCategory = objCategory.walls;
            _objCategory = objCategory.traps;
            _objtypes[2] = (int)objTrap.TrapSmall;
            stageEditor.setAlpha1 = false;
        }
        else if(stageEditor.setAlpha2)
        {
            //_objCategory = objCategory.walls;
            _objCategory = objCategory.traps;
            _objtypes[2] = (int)objTrap.Traper;
            stageEditor.setAlpha2 = false;
        }
        else if(stageEditor.setAlpha3)
        {
            //_objCategory = objCategory.walls;
            _objCategory = objCategory.ballrs;
            _objtypes[1] =// (int)objBallr.B0w;
                            (byte)Random.Range((int)objBallr.B0w, (int)objBallr.B4g2);
            stageEditor.setAlpha3 = false;
        }

        
        else if(stageEditor.setBackQuote)
        {
            //_objCategory = objCategory.etc;
            //_objtypes[0] = (int)objEtc.Finish;
            _objCategory = objCategory.walls;
            _objtypes[0] = (int)objWall.WZb;
            stageEditor.setBackQuote = false;
        }

        _ui = (stageEditor)target;
        onInspectorGUI_fileIO();
        base.OnInspectorGUI();
        onInspectorGUI_selectType();
        onInspectorGUI_selectCategory();
        
        switch(_objCategory) {
            case objCategory.ballrs: onInspectorGUI_Ballers(_ui.CurCell); break;
        }

        onInspectorGUI_history();
        //onInspectorGUI_randomhelper();
    //    onInspectorGUI_addBlck();
        
    }
    
    void onInspectorGUI_addBlck()
    {
        if (GUILayout.Button("Save addBlck"))
        {
            for(int i=101; i<jsons.NumStage(); ++i)
            {
                _ui.ReadJson(jsons.GetStageJson(i));

                for(int x=1; x<cells.MaxX-1;++x)
                for(int z=20+RandEx.Get(4); z<endfield.z-3-RandEx.Get(5);++z)
                {
                    cell c = cells.Get(x,z);
                    if(!c.IsPath)
                        continue;
                    if(RandEx.TruePerCt(30 - (int)(i/33)))
                        continue;
                    if(c.North().IsPath && c.South().IsPath &&
                        c.East().IsPath && c.West().IsPath)
                    {  
                        c.SetWall(cell.type.WZb);
                    }
                }

                FileIO.OnPrj.CreateFile_stage(jsons.GetStageName(i), _ui.CreateJson(stageEditor.saveType.normal, _randType), "txt");
  
            }
        }
    }

    void onInspectorGUI_history()
    {
        GUILayout.BeginHorizontal();
        int idx = (int)_ui.select;
        if(_ui.select <= stageEditor.SelectType.Copy2 )
        {
            GUILayout.Label(string.Format("{0}x{1}", _ui._gaps[idx].x, _ui._gaps[idx].z));
            
        }
        GUILayout.EndHorizontal();
    }

    bool _randomhalper = true;
    string[] shapes = new string[12]{ "◆","■","▲","▼","▣",     "◈","▥","▤","▨","▧",  "X","+"};
    string[] shapes2 = new string[14]{"ㄱ","ㄴ","ㄷ","ㄹ","ㅁ",  "ㅂ","ㅅ","ㅇ","ㅈ","ㅊ",  "ㅋ","ㅌ","ㅍ","ㅎ"};
    string[] shapes3 = new string[10]{"ㅏ","ㅑ","ㅓ","ㅕ","ㅗ",  "ㅛ","ㅜ","ㅠ","ㅡ","ㅣ"};
    int idx0=0, idx1=0, idx2=0, idxRdW=0, idxRdH=0, idx5=0;
    void onInspectorGUI_randomhelper()
    {
        GUILayout.BeginHorizontal();
        bool b = EditorGUILayout.Foldout(_randomhalper, "[random helper]");
        switch(idx5)
        {
            case 0:
            GUILayout.Label(string.Format("{0} stage: [ {4} x {5} ] {1} {2} {3}  ", _stageIdx, shapes[idx0], shapes2[idx1], shapes3[idx2], idxRdW==0?"free":idxRdW.ToString(), idxRdH==0?"free":idxRdH.ToString()) );
            break;
            case 1:
            GUILayout.Label(string.Format("{0} stage: [ {4} x {5} ] {1} {3} {2}  ", _stageIdx, shapes[idx0], shapes2[idx1], shapes3[idx2], idxRdW==0?"free":idxRdW.ToString(), idxRdH==0?"free":idxRdH.ToString()) );
            break;
            case 2:
            GUILayout.Label(string.Format("{0} stage: [ {4} x {5} ] {2} {3} {1}  ", _stageIdx, shapes[idx0], shapes2[idx1], shapes3[idx2], idxRdW==0?"free":idxRdW.ToString(), idxRdH==0?"free":idxRdH.ToString()) );
            break;
            case 3:
            GUILayout.Label(string.Format("{0} stage: [ {4} x {5} ] {2} {1} {3}  ", _stageIdx, shapes[idx0], shapes2[idx1], shapes3[idx2], idxRdW==0?"free":idxRdW.ToString(), idxRdH==0?"free":idxRdH.ToString()) );
            break;
            case 4:
            GUILayout.Label(string.Format("{0} stage: [ {4} x {5} ] {3} {1} {2}  ", _stageIdx, shapes[idx0], shapes2[idx1], shapes3[idx2], idxRdW==0?"free":idxRdW.ToString(), idxRdH==0?"free":idxRdH.ToString()) );
            break;
            default:
            GUILayout.Label(string.Format("{0} stage: [ {4} x {5} ] {3} {2} {1}  ", _stageIdx, shapes[idx0], shapes2[idx1], shapes3[idx2], idxRdW==0?"free":idxRdW.ToString(), idxRdH==0?"free":idxRdH.ToString()) );
            break;
        }
        
        GUILayout.EndHorizontal();

        if(_ui.created || _randomhalper != b)
        {
            idx0 = UnityEngineEx.RandEx.GetN(12);
            idx1 = UnityEngineEx.RandEx.GetN(14);
            idx2 = UnityEngineEx.RandEx.GetN(10);
            idxRdW = UnityEngineEx.RandEx.GetN(4);
            idxRdH = UnityEngineEx.RandEx.GetN(4);
            idx5 = UnityEngineEx.RandEx.GetN(6);
            _ui.created = false;
        }
        _randomhalper = b;
    }

    enum ballrEdit:byte{addPt, addEnd, /*removePt*/}
    ballrEdit _ballrEdit;
    cell _lastBaller = null;
    void onInspectorGUI_Ballers(cell curcell)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.Foldout(true, "[ballr data]"); 
        _ballrEdit = (ballrEdit)EditorGUILayout.EnumPopup(_ballrEdit);
        GUILayout.EndHorizontal();
    
        if(curcell != null && curcell.IsBallr())
            _lastBaller = curcell;
        if(_lastBaller == null || !_lastBaller.IsBallr())
            return;
        
        baller.datas[_lastBaller.ddx].Type = (baller.data.type)EditorGUILayout.EnumPopup(baller.datas[_lastBaller.ddx].Type);
      
            GUILayout.Label("ddx: "+_lastBaller.ddx);
        if (GUILayout.Button("add"))
        {
            switch(_ballrEdit)
            {
                case ballrEdit.addEnd:
                curcell.SetPath((cell.type)(17 + (int)_lastBaller.Type));
                baller.datas[_lastBaller.ddx].pts.Add(new f2(curcell.ct.x, curcell.ct.z));
                baller.datas[_lastBaller.ddx].SetEnd(curcell);
                baller.datas[_lastBaller.ddx].Type = baller.data.type.way1;
                hero.culling.Refresh();
                break;
                case ballrEdit.addPt:
                baller.datas[_lastBaller.ddx].pts.Add(new f2(curcell.ct.x, curcell.ct.z));
                break;
            }
        }
    }
    
    void onInspectorGUI_selectCategory()
    {
        GUILayout.BeginHorizontal();
        _objCategory = (objCategory)EditorGUILayout.EnumPopup("walls, ballrs, traps, etc", _objCategory);
        switch(_objCategory) {
            case objCategory.walls: popupObjType(0, (objWall)_objtypes[0], (int)objWall.W0w, (int)objWall.WZb); break;
            case objCategory.ballrs:popupObjType(1, (objBallr)_objtypes[1], (int)objBallr.B0w, (int)objBallr.BZb); break;
            case objCategory.traps: popupObjType(2, (objTrap)_objtypes[2], (int)objTrap.TrapSmall, (int)objTrap.TraperR); break;
            case objCategory.etc:   popupObjType(3, (objEtc)_objtypes[3], (int)objEtc.Path, (int)objEtc.Food); break;
            case objCategory.balls: popupObjType(4, (objBall)_objtypes[4], (int)objBall.B0w, (int)objBall.BZb); break;
        }
        GUILayout.EndHorizontal();
    }
    
    void onInspectorGUI_selectType()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("select type"); GUILayout.Label("");
        if(_objCategory == objCategory.walls || _objCategory == objCategory.etc)
        {
            _ui.select = (stageEditor.SelectType)EditorGUILayout.EnumPopup(_ui.select);
        }
        else
        {
            GUILayout.Label("single");
            _ui.select = stageEditor.SelectType.Single;
        }
        GUILayout.EndHorizontal();
    }


    enum objWall:byte{
        W0w = cell.type.W0w, W1y, W2o, W3g1, W4g2, W5s, W6b, W7pu, W8pi, W9r, WZb,}

    enum objBallr:byte{ 
        B0w= cell.type.Br0w, B1y, B2o, B3g1, B4g2, B5s, B6b, B7pu, B8pi, B9r, BZb,}

    enum objTrap:byte{
        TrapSmall = cell.type.TrapSmall, 
        Traper = cell.type.Traper,
        TraperI = cell.type.TraperI,
        TraperL = cell.type.TraperL,
        TraperR = cell.type.TraperR, }

    enum objEtc:byte{
        Path = cell.type.Path, 
        Finish = cell.type.Finish, 
        Food = cell.type.Food, 
    }
    
    enum objBall:byte{ 
        B0w= cell.type.B0w, B1y, B2o, B3g1, B4g2, B5s, B6b, B7pu, B8pi, B9r, BZb,}


    stageEditor.saveType _saveType = stageEditor.saveType.normal;
    stageEditor.randType _randType = stageEditor.randType.r0;
    enum ioType:byte{onRes, onPrj, onPrjF5, onResShuffle2, onResShuffle, onResF5  }
    ioType _ioType = ioType.onRes;
    string _editFileName;
    int _stageIdx;
    void onInspectorGUI_fileIO()
    {
        GUILayout.BeginHorizontal();
        
        if(_ui.gameObject.activeSelf && GUILayout.Button("Menu"))
            _ui.gameObject.SetActive(false);
        else if(!_ui.gameObject.activeSelf)
        {
            if (GUILayout.Button("Edit onRes"))
            {
                _ioType = ioType.onRes;
                _ui.gameObject.SetActive(true);
            }
            if (GUILayout.Button("Edit onPrj"))
            {
                _ioType = ioType.onPrj;
                _ui.gameObject.SetActive(true);
            }
        }
        _saveType = (stageEditor.saveType)EditorGUILayout.EnumPopup(_saveType);
        _randType = (stageEditor.randType)EditorGUILayout.EnumPopup(_randType);

        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        _ioType = (ioType)EditorGUILayout.EnumPopup(_ioType);
        switch(_ioType)
        {
            case ioType.onRes:
            if (GUILayout.Button("Load"))
            {
                _stageIdx = int.Parse(_editFileName);
                _ui.ReadJson(jsons.GetStageJson(_stageIdx));
            }
            if (GUILayout.Button("Save"))
            {
                _stageIdx = int.Parse(_editFileName);
                FileIO.OnRes.CreateFile_stage(jsons.GetStageName(_stageIdx), _ui.CreateJson(stageEditor.saveType.normal, stageEditor.randType.r0), "txt");
            }
            break;
            case ioType.onPrj:
            if (GUILayout.Button("Load"))
                _ui.ReadJson(FileIO.OnPrj.ReadStage(_editFileName, "txt"));  //    ui_panel.ReadJSON(FileIO.Local.Read(jsData.editFileName, "txt"), js.SeriType.WithLoad);
            if (GUILayout.Button("Save"))
                FileIO.OnPrj.CreateFile_stage(_editFileName, _ui.CreateJson(_saveType, _randType), "txt");
            break;
            case ioType.onPrjF5:
            if (GUILayout.Button("Save"))
            {
                FileIO.OnPrj.CreateFile_stage(_editFileName+"_", _ui.CreateJson(stageEditor.saveType.normal, _randType), "txt");
                FileIO.OnPrj.CreateFile_stage(_editFileName+"_c", _ui.CreateJson(stageEditor.saveType.shift, _randType), "txt");
                FileIO.OnPrj.CreateFile_stage(_editFileName+"_i", _ui.CreateJson(stageEditor.saveType.reverse, _randType), "txt");
                FileIO.OnPrj.CreateFile_stage(_editFileName+"_p", _ui.CreateJson(stageEditor.saveType.decalFrLeft, _randType), "txt");
                FileIO.OnPrj.CreateFile_stage(_editFileName+"_q", _ui.CreateJson(stageEditor.saveType.decalFrRight, _randType), "txt");
            }
            break;
            
            case ioType.onResShuffle2:
            if (GUILayout.Button("Save"))
            {
                int count  = 0;
                for(int i85 = 0; i85 < 7; ++i85)
                {
                    for(int i=0; i<5; ++i)
                    {
                        for(int i5=0; i5<17; ++i5)
                        {
                            int idx = i85*85 + i5*5 + i;
                            if(idx >= jsons.NumStage())
                                break;
                            ++count;
                            string str = jsons.GetStageJson(idx);
                            FileIO.OnPrj.CreateFile_stage(count+" " + jsons.GetStageName(idx), str, "txt");
                        }
                        
                    }
                }
            }
            break;
            
            
            case ioType.onResShuffle:
            if (GUILayout.Button("Save"))
            {
                List<int> sdf = new List<int>();
                for(int i=0; i<jsons.NumStage(); ++i)
                    sdf.Add(i);
                for(int i=0; i<sdf.Count; i+=5)
                    Shuffle(sdf, i, 5);

                for(int i=0; i<sdf.Count; ++i)
                {
                    string str = jsons.GetStageJson(sdf[i]);
                    FileIO.OnPrj.CreateFile_stage(i+" " + jsons.GetStageName(sdf[i]), str, "txt");
                }
            }
            break;
            case ioType.onResF5:
            if (GUILayout.Button("Save"))
            {
                for(int i=0; i<=int.Parse(_editFileName); ++i)
                {
                    _ui.ReadJson(jsons.GetStageJson(i));
                    FileIO.OnPrj.CreateFile_stage(i+"_", _ui.CreateJson(stageEditor.saveType.normal, _randType), "txt");
                    FileIO.OnPrj.CreateFile_stage(i+"_c", _ui.CreateJson(stageEditor.saveType.shift, _randType), "txt");
                    FileIO.OnPrj.CreateFile_stage(i+"_i", _ui.CreateJson(stageEditor.saveType.reverse, _randType), "txt");
                    FileIO.OnPrj.CreateFile_stage(i+"_p", _ui.CreateJson(stageEditor.saveType.decalFrLeft, _randType), "txt");
                    FileIO.OnPrj.CreateFile_stage(i+"_q", _ui.CreateJson(stageEditor.saveType.decalFrRight, _randType), "txt");
                }
            }
            break;
        }
        _editFileName = EditorGUILayout.TextField(_editFileName);
        GUILayout.EndHorizontal();
        
    }

    public static void Shuffle(IList<int> array, int first, int count)
    {
        for (int n = count; n > 1;)
        {
            int k = Random.Range(0, count);
            --n;
            int temp = array[first+n];
            array[first+n] = array[first+k];
            array[first+k] = temp;
        }
    }
    //

    void popupObjType(int idx, System.Enum e, int min, int max)
    {
        _objtypes[idx] = (byte)Mathf.Clamp((int)EditorGUILayout.EnumPopup(e).GetHashCode(), min, max);
        _ui.SetType((cell.type)_objtypes[idx]);
    }
    
}

