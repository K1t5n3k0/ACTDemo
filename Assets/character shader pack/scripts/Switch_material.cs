using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyGameStudio.character_shader_pack_3_0
{
    public class Switch_material : MonoBehaviour
    {
        public Material[] materials;

        public Renderer render;

        private int index = 0;

        void OnEnable()
        {
            this.index = 0;

            this.render.material = this.materials[this.index];

            if (Demo_control.instance != null)
            {
                Demo_control.instance.change_title(this.index);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.index++;
                if (this.index >= this.materials.Length)
                    this.index = 0;

                this.render.material = this.materials[this.index];

                Demo_control.instance.play_btn_sound();

                Demo_control.instance.change_title(this.index);
            }

            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.index--;
                if (this.index <= -1)
                    this.index = (this.materials.Length - 1);

                this.render.material = this.materials[this.index];

                Demo_control.instance.play_btn_sound();

                Demo_control.instance.change_title(this.index);
            }
        }
    }
}
