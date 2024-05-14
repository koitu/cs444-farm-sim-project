using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Plant : MonoBehaviour
{
    public enum MeshNames {
        L_Stage,
        M_Stage,
        S_Stage,
        Seed,
        Default,
        Group
    }

    public Mesh GrowthStage_L;
    public Mesh GrowthStage_M;
    public Mesh GrowthStage_S;
    public Mesh Seed;

    public Mesh Default;
    public Mesh Group;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public bool isGrowing = false;
    private int stageN = 0;
    private int maxStageN = 3;

    public string PlantName { get; protected set; }
    public int GrowTime { get; protected set; }
    public int GrowTimeLeft { get; set; }


    public abstract void Breed(Plant partner);

    public virtual void StartGrowing()
    {
        this.GrowTimeLeft = this.GrowTime;
        this.isGrowing = true;
        this.meshCollider.enabled = false;
    }
    
    public virtual void GrowingStep()
    {
        this.GrowTimeLeft--;
        if (this.GrowTimeLeft <= 0)
        {
            this.GrowTimeLeft = this.GrowTime;
            if (this.stageN < maxStageN)
            {

                this.stageN++;
            } else
            {
                this.meshCollider.enabled = true;
                this.stageN = maxStageN;
            }
        }
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public virtual void SetMesh(MeshNames meshType)
    {
        switch(meshType) {
            case MeshNames.Group:
                this.meshFilter.mesh = this.Group;
                this.meshCollider.sharedMesh = this.Group;
                break;
            case MeshNames.Seed:
                this.meshFilter.mesh = this.Seed;
                this.meshCollider.sharedMesh = this.Seed;
                break;
            case MeshNames.S_Stage:
                this.meshFilter.mesh = this.GrowthStage_S;
                this.meshCollider.sharedMesh = this.GrowthStage_S;
                break;
            case MeshNames.M_Stage:
                this.meshFilter.mesh = this.GrowthStage_M;
                this.meshCollider.sharedMesh = this.GrowthStage_M;
                break;
            case MeshNames.L_Stage:
                this.meshFilter.mesh = this.GrowthStage_L;
                this.meshCollider.sharedMesh = this.GrowthStage_L;
                break;
            default:
                this.meshFilter.mesh = this.Default;
                this.meshCollider.sharedMesh = this.Default;
                break;

        }
    }

    public override string ToString()
    {
        return $"{PlantName} with {GrowTimeLeft} days left to grow.";
    }

    private void Start()
    {
        gameObject.tag = "Plant";
        this.meshFilter = this.GetComponent<MeshFilter>();
        this.meshCollider = this.GetComponent<MeshCollider>();
    }

    private void Update()
    {
        if (isGrowing)
        {
            switch (stageN)
            {
                case 0:
                    SetMesh(MeshNames.Seed);
                    break;
                case 1:
                    SetMesh(MeshNames.S_Stage);
                    this.meshFilter.mesh = this.GrowthStage_S;
                    this.meshCollider.sharedMesh = this.GrowthStage_S;
                    break;
                case 2:
                    SetMesh(MeshNames.M_Stage);
                    this.meshFilter.mesh = this.GrowthStage_M;
                    this.meshCollider.sharedMesh = this.GrowthStage_M;
                    break;
                case 3:
                    SetMesh(MeshNames.L_Stage);
                    this.meshFilter.mesh = this.GrowthStage_L;
                    this.meshCollider.sharedMesh = this.GrowthStage_L;
                    this.isGrowing = false;
                    break;
                default:
                    SetMesh(MeshNames.Default);
                    this.meshFilter.mesh = this.Default;
                    this.meshCollider.sharedMesh = this.Default;
                    break;
            }
        }
    }
}
