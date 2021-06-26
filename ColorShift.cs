using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR
{
    /// <summary>
    /// 
    /// </summary>
    public class ColorShift : UdonSharpBehaviour 
    {
        [Tooltip("The object to apply ColorShift to.")]
        public Renderer TargetObj;
        [Tooltip("Target colors to use for cycling.")]
        public Color32[] Colors = { Color.red, Color.magenta, Color.blue, Color.cyan, Color.green, new Color(1,1,0,1)};
        [Tooltip("Typically you want this set to '_Color' or '_EmissionColor'")]
        public string TargetProperty = "_Color";
        [Tooltip("Ammount of time in secconds it takes to transistion to the next color.")]
        public float TargetTime = 3;
        [Tooltip("Makes it so the colors randomly cycle.")]
        public bool isRandom;
        public bool isShared;

        private Material _mat;

        private int _CurrentColor = 0;
        private int _TargetColor = 1;
        private int _OldColor = -1;
        private int _NextColor = -1;

        private float _R;
        private float _G;
        private float _B;
        private float _A;

        private float _r;
        private float _g;
        private float _b;
        private float _a;

        private float _tr;
        private float _tg;
        private float _tb;
        private float _ta;

        private float _Lerp;

        private Color _tempColor;

        public void ToggleRandom()
        {
            isRandom = !isRandom;
        }

        public void Start()
        {
            if (TargetObj == null)
            {
                TargetObj = gameObject.GetComponent<Renderer>();
            }
            if (isShared)
            {
                _mat = TargetObj.sharedMaterial;
            } else
            {
                _mat = TargetObj.material;
            }
            _r = Colors[_CurrentColor].r;
            _g = Colors[_CurrentColor].g;
            _b = Colors[_CurrentColor].b;
            _a = Colors[_CurrentColor].a;
        }

        public void Update()
        {
            if (_tr == _R && _tg == _G && _tb == _B && _ta == _A)
            {
                NextColor();
            } else
            {
                Debug.LogWarning((Time.deltaTime / TargetTime).ToString());
                _Lerp = _Lerp + (Time.deltaTime / TargetTime);
                _tr = Mathf.Lerp(_r, _R, _Lerp);
                _tg =Mathf.Lerp(_g, _G, _Lerp);
                _tb =Mathf.Lerp(_b, _B, _Lerp);
                _ta =Mathf.Lerp(_a, _A, _Lerp);
            }
            _tr = Mathf.RoundToInt(_tr);
            _tg = Mathf.RoundToInt(_tg);
            _tb = Mathf.RoundToInt(_tb);
            _ta = Mathf.RoundToInt(_ta);
            Debug.Log("Lerp: " + _Lerp + " R: " + _tr + " G: " + _tg + " B: " + _tb + " A: " + _ta);
            _tempColor = new Color32((byte) (_tr),(byte) (_tg),(byte) (_tb),(byte) (_ta));
            _mat.SetColor(TargetProperty, _tempColor);
        }

        public void NextColor()
        {
            _OldColor = _TargetColor;
            _Lerp = 0;
            if (isRandom)
            {
                _PickRandom();
            } else
            {
                _NextColor = _TargetColor + 1;
                if (_NextColor >= Colors.Length) _NextColor = 0;
            }
            _TargetColor = _NextColor;
            InitColor();
        }
        private void _PickRandom()
        {
            _NextColor = Random.Range(0, Colors.Length);
            if (_NextColor == _OldColor)
            {
                Debug.Log("Random Pick Failed, Picking new color.");
                _PickRandom();
            }
            return;
        }
        private void InitColor()
        {
            Debug.Log("Setting Target Color to: " + _TargetColor);
            _r = Colors[_OldColor].r;
            _g = Colors[_OldColor].g;
            _b = Colors[_OldColor].b;
            _a = Colors[_OldColor].a;

            _R = Colors[_TargetColor].r;
            _G = Colors[_TargetColor].g;
            _B = Colors[_TargetColor].b;
            _A = Colors[_TargetColor].a;
        }
    }
}
