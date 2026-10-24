using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR.Editor {
public static class GameArtImporter {
    const string Root="Assets/LocalGameAssets/Resources/BWVRArt";
    [Serializable] sealed class Bundle { public Model[] models; public TextureData grass; }
    [Serializable] sealed class Model { public string name; public Primitive[] primitives; public TextureData[] textures; }
    [Serializable] sealed class Primitive { public float[] vertices; public int[] indices; public long skin; public int[] color; public Pose[] poses; }
    [Serializable] sealed class Pose { public string name; public float[] vertices; }
    [Serializable] sealed class TextureData { public long id; public int width,height; public int[] rgba; }
    [MenuItem("BWVR/Import local game artwork")]
    public static void ImportDefault()=>Import(Path.GetFullPath(".local/game-art.json"));
    public static void Import(string path) {
        if(Application.isPlaying)throw new InvalidOperationException("Stop Play Mode before importing artwork.");
        var bundle=JsonUtility.FromJson<Bundle>(File.ReadAllText(path));
        if(bundle?.models==null||bundle.models.Length!=9)throw new InvalidDataException("Expected nine converted game models.");
        Directory.CreateDirectory(Root);AssetDatabase.Refresh();
        var catalog=AssetDatabase.LoadAssetAtPath<GameArtCatalog>(Root+"/Catalog.asset");
        if(catalog==null){catalog=ScriptableObject.CreateInstance<GameArtCatalog>();AssetDatabase.CreateAsset(catalog,Root+"/Catalog.asset");}
        var textures=new Dictionary<long,Texture2D>();int triangles=0;
        foreach(var model in bundle.models) {
            if(model.name is not ("cow" or "house1" or "house2" or "house3" or "woman" or "man" or "beech" or "oak" or "pine"))throw new InvalidDataException("Unknown model key");
            foreach(var data in model.textures)if(!textures.ContainsKey(data.id))textures[data.id]=Texture(data);
            var root=new GameObject(model.name);var geometry=new GameObject("Original game mesh");geometry.transform.SetParent(root.transform,false);
            try {
                for(int part=0;part<model.primitives.Length;part++) {
                    var p=model.primitives[part];if(p.vertices.Length%8!=0||p.indices.Length%3!=0)throw new InvalidDataException("Malformed mesh buffers");
                    int count=p.vertices.Length/8;var vertices=new Vector3[count];var normals=new Vector3[count];var uv=new Vector2[count];
                    for(int i=0;i<count;i++){int n=i*8;vertices[i]=new Vector3(p.vertices[n],p.vertices[n+1],-p.vertices[n+2]);normals[i]=new Vector3(p.vertices[n+5],p.vertices[n+6],-p.vertices[n+7]);uv[i]=new Vector2(p.vertices[n+3],p.vertices[n+4]);}
                    // Reflect Z for Unity's forward direction; reverse triangle winding with it.
                    var indices=(int[])p.indices.Clone();for(int i=0;i<indices.Length;i+=3){(indices[i],indices[i+1])=(indices[i+1],indices[i]);}
                    var mesh=new Mesh{name=model.name+"_"+part,indexFormat=count>65535?IndexFormat.UInt32:IndexFormat.UInt16};mesh.vertices=vertices;mesh.normals=normals;mesh.uv=uv;mesh.triangles=indices;mesh.RecalculateBounds();mesh.RecalculateTangents();
                    if(p.poses!=null){var weights=new BoneWeight[count];for(int i=0;i<count;i++)weights[i]=new BoneWeight{boneIndex0=0,weight0=1};mesh.boneWeights=weights;mesh.bindposes=new[]{Matrix4x4.identity};}
                    if(p.poses!=null)foreach(var pose in p.poses){
                        if(pose.vertices.Length!=p.vertices.Length)throw new InvalidDataException("Pose vertex count mismatch");
                        var delta=new Vector3[count];var deltaNormals=new Vector3[count];
                        for(int i=0;i<count;i++){int n=i*8;delta[i]=new Vector3(pose.vertices[n],pose.vertices[n+1],-pose.vertices[n+2])-vertices[i];deltaNormals[i]=new Vector3(pose.vertices[n+5],pose.vertices[n+6],-pose.vertices[n+7])-normals[i];}
                        mesh.AddBlendShapeFrame(pose.name,100,delta,deltaNormals,null);
                    }
                    mesh=Store(mesh,Root+"/"+mesh.name+".asset");triangles+=indices.Length/3;
                    var shader=Shader.Find("BWVR/GameSurface");if(shader==null)throw new InvalidOperationException("BWVR game surface shader did not compile");
                    var material=new Material(shader){name=model.name+"_material_"+part};
                    if(textures.TryGetValue(p.skin,out var texture)){material.mainTexture=texture;material.color=Color.white;}
                    else material.color=new Color(p.color[0]/255f,p.color[1]/255f,p.color[2]/255f,p.color[3]/255f);
                    material=Store(material,Root+"/"+material.name+".mat");
                    var child=new GameObject("Part "+part);child.transform.SetParent(geometry.transform,false);
                    if(p.poses!=null){var skin=child.AddComponent<SkinnedMeshRenderer>();skin.sharedMesh=mesh;skin.sharedMaterial=material;skin.bones=new[]{geometry.transform};skin.rootBone=geometry.transform;skin.localBounds=new Bounds(mesh.bounds.center,mesh.bounds.size*1.4f);}
                    else{child.AddComponent<MeshFilter>().sharedMesh=mesh;child.AddComponent<MeshRenderer>().sharedMaterial=material;}
                }
                var renderers=root.GetComponentsInChildren<Renderer>();if(renderers.Length==0)throw new InvalidDataException("Empty model");
                var bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
                if(bounds.size.y<=.0001f)throw new InvalidDataException("Model has no height");
                geometry.transform.localScale=Vector3.one/bounds.size.y;
                geometry.transform.localPosition=new Vector3(-bounds.center.x,-bounds.min.y,-bounds.center.z)/bounds.size.y;
                var collider=root.AddComponent<BoxCollider>();collider.center=new Vector3(0,.5f,0);collider.size=bounds.size/bounds.size.y;
                if(model.name=="cow"&&model.primitives[0].poses!=null)root.AddComponent<OriginalCowMotion>();
                var prefab=PrefabUtility.SaveAsPrefabAsset(root,Root+"/"+model.name+".prefab");
                var serialized=new SerializedObject(catalog);serialized.FindProperty(model.name).objectReferenceValue=prefab;serialized.ApplyModifiedPropertiesWithoutUndo();
            } finally {UnityEngine.Object.DestroyImmediate(root);}
        }
        var grassTexture=Texture(bundle.grass);var grass=new Material(Shader.Find("BWVR/GameSurface")){name="Original grass",mainTexture=grassTexture};grass.mainTextureScale=Vector2.one*6;
        catalog.grass=Store(grass,Root+"/Grass.mat");EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssets();AssetDatabase.Refresh();
        Debug.Log("BWVR_ART_IMPORTED: nine original models, "+triangles+" triangles; local assets excluded from Git.");
    }
    static Texture2D Texture(TextureData t) {
        if(t.width<=0||t.height<=0||t.width>4096||t.height>4096||t.rgba.Length!=t.width*t.height*4)throw new InvalidDataException("Invalid texture dimensions");
        var bytes=new byte[t.rgba.Length];for(int i=0;i<bytes.Length;i++){if((uint)t.rgba[i]>255)throw new InvalidDataException("Invalid texel");bytes[i]=(byte)t.rgba[i];}
        var texture=new Texture2D(t.width,t.height,TextureFormat.RGBA32,true){name="Game texture "+t.id,wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear};texture.SetPixelData(bytes,0);texture.Apply(true,false);
        return Store(texture,Root+"/Texture_"+t.id+".asset");
    }
    static T Store<T>(T value,string path) where T:UnityEngine.Object {
        var existing=AssetDatabase.LoadAssetAtPath<T>(path);
        if(existing==null){AssetDatabase.CreateAsset(value,path);return value;}
        EditorUtility.CopySerialized(value,existing);EditorUtility.SetDirty(existing);UnityEngine.Object.DestroyImmediate(value);return existing;
    }
}
}
