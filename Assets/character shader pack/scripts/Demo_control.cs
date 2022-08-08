using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace EasyGameStudio.character_shader_pack_3_0
{
    public class Demo_control : MonoBehaviour
    {
        public static Demo_control instance;

        public int index;
        public GameObject[] gameobjects;

        [Header("title")]
        public Text text_title;
        public string[] titles;

        //public Volume m_volume;

        public AudioSource audio_source;
        public AudioClip ka;

        void Awake()
        {
            Demo_control.instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.change_to_index();
        }

        public void on_next_btn()
        {
            this.index++;
            if (this.index >= this.gameobjects.Length)
                this.index = 0;

            this.change_to_index();
        }

        public void on_previous_btn()
        {
            this.index--;
            if (this.index < 0)
                this.index = this.gameobjects.Length - 1;

            this.change_to_index();
        }

        private void change_to_index()
        {
            this.text_title.text = this.titles[this.index];

            for (int i = 0; i < this.gameobjects.Length; i++)
            {
                if (this.index == i)
                {
                    this.gameobjects[i].SetActive(true);
                }
                else
                {
                    this.gameobjects[i].SetActive(false);
                }
            }

            //float value = 9.5f;

            //if (this.index == 0|| this.index == 3 || this.index == 5 || this.index == 6 || this.index == 8 || this.index == 9)
            //    value = 8.8f;

            //if (this.m_volume.profile.TryGet<Exposure>(out var exposure))
            //{
            //    exposure.fixedExposure.overrideState = true;
            //    exposure.fixedExposure.value = value;
            //}

            this.audio_source.PlayOneShot(this.ka);
        }

        public void change_title(int index)
        {
            this.text_title.text = this.titles[this.index] + "-" + index;
        }

        public void play_btn_sound()
        {
            this.audio_source.PlayOneShot(this.ka);
        }
    }
}