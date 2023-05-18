using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LionStudios.Suite.Creatives
{

    public static partial class LionDebug
    {

        private static void Color(float r, float g, float b, string outKey)
        {
            Store(outKey, new Color(r, g, b));
        }
        
        private static void Color(float r, float g, float b, float a, string outKey)
        {
            Store(outKey, new Color(r, g, b, a));
        }

        private static void SetMaterial(string rendererKey, string materialKey)
        {
            GetStoredComp<Renderer>(rendererKey).material = GetStored<Material>(materialKey);
        }
        
        private static void SetMaterial(string rendererKey, int index, string materialKey)
        {
            GetStoredComp<Renderer>(rendererKey).materials[index] = GetStored<Material>(materialKey);
        }
        
        private static void SetColor(string rendererKey, Color color)
        {
            object obj = GetStoredComp(rendererKey, typeof(SpriteRenderer), typeof(Graphic), typeof(Renderer));
            
            if (obj is SpriteRenderer sr)
                sr.color = color;
            else if (obj is Graphic g)
                g.color = color;
            else if (obj is Renderer r)
                r.material.SetColor("_Color", color);
            else
                throw new ArgumentException("Wrong type");
        }
        
        private static void SetColor(string rendererKey, string name, Color color)
        {
            GetStoredComp<Renderer>(rendererKey).material.SetColor(name, color);
        }
        
        private static void SetColor(string rendererKey, int matIndex, string name, Color color)
        {
            GetStoredComp<Renderer>(rendererKey).materials[matIndex].SetColor(name, color);
        }
        
        private static void SetSprite(string rendererKey, string spriteKey)
        {
            object obj = GetStoredComp(rendererKey, typeof(SpriteRenderer), typeof(Image));
            if (obj is SpriteRenderer sr)
                sr.sprite = GetStored<Sprite>(spriteKey);
            else if (obj is Image img)
                img.sprite = GetStored<Sprite>(spriteKey);
            else
                throw new ArgumentException("Wrong type");
        }
        
        private static void SetMesh(string rendererOrFilterKey, string meshKey)
        {
            object obj = GetStoredComp(rendererOrFilterKey, typeof(SkinnedMeshRenderer), typeof(MeshFilter));
            
            if (obj is SkinnedMeshRenderer smr)
                smr.sharedMesh = GetStored<Mesh>(meshKey);
            else if (obj is MeshFilter mf)
                mf.sharedMesh = GetStored<Mesh>(meshKey);
            else
                throw new ArgumentException("Wrong type");
        }
        
        private static void SetText(string textCompKey, string text)
        {
            object obj = GetStoredComp(textCompKey, typeof(Text), typeof(TextMesh));
            
            if (obj is Text txt)
                txt.text = text;
            else if (obj is TextMesh tm)
                tm.text = text;
            // else if (obj is TMP tmp)
            //     tmp.text = text;
            else
                throw new ArgumentException("Wrong type");
        }


        [RuntimeInitializeOnLoadMethod]
        public static void AddRenderingCommands()
        {
            AddCommand<float, float, float, string>("color", "Creates and stores a Color object", Color);
            AddCommand<float, float, float, float, string>("color", "Creates and stores a Color object", Color);
            AddCommand<string, string>("setmaterial", "", SetMaterial);
            AddCommand<string, int, string>("setmaterial", "", SetMaterial);
            AddCommand<string, Color>("setcolor", "", SetColor);
            AddCommand<string, string, Color>("setcolor", "", SetColor);
            AddCommand<string, int, string, Color>("setcolor", "", SetColor);
            AddCommand<string, string>("setmesh", "", SetMesh);
            AddCommand<string, string>("setsprite", "", SetSprite);
            AddCommand<string, string>("settext", "", SetText);
        }

    }

}