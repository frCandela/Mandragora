using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Obi;

[RequireComponent(typeof(ObiSolver))]
public class Test : MonoBehaviour
{

    ObiSolver solver;

    Obi.ObiSolver.ObiCollisionEventArgs collisionEvent;

    void Awake()
    {
        solver = GetComponent<Obi.ObiSolver>();
    }

    void OnEnable()
    {
        solver.OnCollision += Solver_OnCollision;
    }

    void OnDisable()
    {
        solver.OnCollision -= Solver_OnCollision;
    }

    public int test = 0;
    void Solver_OnCollision(object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
    {


        foreach (Oni.Contact contact in e.contacts)
        {
            // this one is an actual collision:
            if (contact.distance < 0.01)
            {
                Component component;
                if (ObiCollider.idToCollider.TryGetValue(contact.other, out component))
                {
                    ObiSolver.ParticleInActor pa = solver.particleToActor[contact.particle];
                    ObiEmitter emitter = pa.actor as ObiEmitter;
                    if (emitter != null)
                    {
                        emitter.life[pa.indexInActor] = 0;
                        test++;
                    }
                }
            }
        }
    }

}