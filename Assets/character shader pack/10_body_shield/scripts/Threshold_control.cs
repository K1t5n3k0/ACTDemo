using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyGameStudio.character_shader_pack_3_0
{
    public class Threshold_control : MonoBehaviour
    {
        [Header("show speed")]
        [Range(0, 5f)]
        public float speed_show;

        [Header("hide speed")]
        [Range(0, 5f)]
        public float speed_hide;


        private bool is_showing = false;
        private bool is_hiding = false;
        private float threshold = 0;


        [Header("min max threshold")]
        public float min_threshold;
        public float max_threshold;

        [Header("Material")]
        public Material _material;

        [Header("Audio Source and Audio Clip")]
        public AudioSource audio_source;
        public AudioClip audio_clip_show;
        public AudioClip audio_clip_hide;


        // Start is called before the first frame update
        void Start()
        {

        }

        public void reset_material()
        {
            if (this.GetComponent<MeshRenderer>() == null)
            {
                this._material = this.GetComponent<SkinnedMeshRenderer>().material;
            }
            else
            {
                this._material = this.GetComponent<MeshRenderer>().material;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (this.is_showing)
            {
                //this.threshold = Mathf.Lerp(this.threshold, this.min_threshold, Time.deltaTime * this.speed_show);

                this.threshold -= Time.deltaTime * this.speed_show;

                if (this.threshold <= this.min_threshold)
                {
                    this.threshold = this.min_threshold;

                    this.is_showing = false;
                }

                this._material.SetFloat("_threshold", this.threshold);
            }

            if (this.is_hiding)
            {
                //this.threshold = Mathf.Lerp(this.threshold, this.max_threshold, Time.deltaTime * this.speed_show);

                this.threshold += Time.deltaTime * this.speed_hide;

                if (this.threshold >= this.max_threshold)
                {
                    this.threshold = this.max_threshold;

                    this.is_hiding = false;
                }
                this._material.SetFloat("_threshold", this.threshold);
            }
        }

        public void show()
        {
            this.is_hiding = false;

            this.threshold = this.max_threshold;

            this._material.SetFloat("_threshold", this.threshold);

            this.is_showing = true;

            if (this.audio_source != null)
                this.audio_source.PlayOneShot(this.audio_clip_show);
        }

        public void hide()
        {
            this.is_showing = false;

            this.threshold = this.min_threshold;

            this._material.SetFloat("_threshold", this.threshold);

            this.is_hiding = true;

            if (this.audio_source != null)
                this.audio_source.PlayOneShot(this.audio_clip_hide);
        }
    }
}