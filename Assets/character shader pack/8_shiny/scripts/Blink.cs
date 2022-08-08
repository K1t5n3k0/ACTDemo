using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyGameStudio.character_shader_pack_3_0
{
    public class Blink : MonoBehaviour
    {
        [Header("material")]
        public Material material;

        [Header("speed of blink")]
        [Range(0, 5)]
        public float speed_blink;

        private float tint_amount = 1;

        // Update is called once per frame
        void Update()
        {
            if (this.tint_amount <= 0.01)
            {
                this.tint_amount = 1;
            }
            else
            {
                this.tint_amount -= Time.deltaTime * this.speed_blink;
                this.material.SetFloat("_blend_opacity", this.tint_amount);
            }
        }
    }
}
