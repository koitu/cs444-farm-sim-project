using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Plant : MonoBehaviour
{
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
                this.stageN = maxStageN;
            }
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
                    this.meshFilter.mesh = this.Seed;
                    this.meshCollider.sharedMesh = this.Seed;
                    break;
                case 1:
                    
                    this.meshFilter.mesh = this.GrowthStage_S;
                    this.meshCollider.sharedMesh = this.GrowthStage_S;
                    break;
                case 2:
                    this.meshFilter.mesh = this.GrowthStage_M;
                    this.meshCollider.sharedMesh = this.GrowthStage_M;
                    break;
                case 3:
                    this.meshFilter.mesh = this.GrowthStage_L;
                    this.meshCollider.sharedMesh = this.GrowthStage_L;
                    this.isGrowing = false;
                    break;
                default:
                    this.meshFilter.mesh = this.Default;
                    this.meshCollider.sharedMesh = this.Default;
                    break;
            }
        }
    }
}
