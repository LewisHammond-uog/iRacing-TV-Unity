using System;
using TMPro;
using UnityEngine;


public class OverlayPodium : MonoBehaviour
{
		public IPC ipc;
    
        public int index;
        public GameObject enable;
        public TextSettings textLayer1;
        public TextSettings textLayer2;
        public TextSettings textLayer3;

        public ImageSettings[] p1Images;
        public ImageSettings[] p2Images;
        public ImageSettings[] p3Images;
        
        [SerializeField] public TMP_Text p1Given;
        [SerializeField] public TMP_Text p1Family;
        
        [SerializeField] public TMP_Text p2Given;
        [SerializeField] public TMP_Text p2Family;
        
        [SerializeField] public TMP_Text p3Given;
        [SerializeField] public TMP_Text p3Family;

        [NonSerialized] public long indexLiveData;
    
        public bool update = true;
	
    
        public void Update()
        {
        	enable.SetActive( LiveData.Instance.liveDataControlPanel.masterOn && LiveData.Instance.liveDataControlPanel.customLayerOn[ index ] && ipc.isConnected && LiveData.Instance.isConnected );
    
        	if ( indexLiveData != IPC.indexLiveData && update)
        	{
        		indexLiveData = IPC.indexLiveData;
    
        		var liveDataCustom = LiveData.Instance.liveDataCustom[ index ];

                foreach (var p1img in p1Images)
                {
	                p1img.carIdx = LiveData.Instance.liveDataCustom[index].carIdx1;
                }
                
                foreach (var p2img in p2Images)
                {
	                p2img.carIdx = LiveData.Instance.liveDataCustom[index].carIdx2;
                }
                
                foreach (var p3img in p3Images)
                {
	                p3img.carIdx = LiveData.Instance.liveDataCustom[index].carIdx3;
                }

                string[] p1 = liveDataCustom.textLayer1.Split("##");
                string[] p2 = liveDataCustom.textLayer2.Split("##");
                string[] p3 = liveDataCustom.textLayer3.Split("##");
                
                
                p1Given.text = p1.Length > 0 ? p1[0] : "";
                p1Family.text = p1.Length > 1 ? p1[1] : "";
                
                p2Given.text = p2.Length > 0 ? p2[0] : "";
                p2Family.text = p2.Length > 1 ? p2[1] : "";
                
                p3Given.text = p3.Length > 0 ? p3[0] : "";
                p3Family.text = p3.Length > 1 ? p3[1] : "";
            }
        }
}
