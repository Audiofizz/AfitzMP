using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Timers;

namespace GameServer
{
    public enum AnimationStates
    {
        idle,
        running,
        somthing,
        falling,
        swingSword
    }

    public enum CombatAnimations
    {
        Swing,
    }

    class Player : PhysicsObject, Damageable
    {

        #region User Information

        [Header("User Information")]

        public string username;

        [HideInInspector] public int animationState;

        public int score = 0;

        public int deaths = 0;

        public int teamdex = 1;

        #endregion

        #region Modifiers

        [Header("Player Modifiers")]

        [SerializeField] private float baseMoveSpeed = 5f;

        [SerializeField] private float Damage = 10f;

        private float runModifer = 2;

        private float jumpHeight = 20f / Constants.TICKS_PER_SEC;

        #endregion

        #region Cooldowns

        private bool LeftClickExit = false;

        private TimedCallback LeftClickCooldown;

        private bool RightClickExit = false;

        private TimedCallback RightClickCooldown;

        #endregion

        #region Inputs

        private bool canJump = true;

        private bool canShoot = true;

        private bool[] inputs;

        [HideInInspector] public int inputTick;

        private bool Attacking = false;

        #endregion

        #region Stats

        [Header("Player Stats")]

        public float health = 100;

        public int maxHealth = 100;

        #endregion

        [Header("Body Parts")]

        public Transform Head;

        public Transform ShootLocation;

        public Transform HandLocation;

        [HideInInspector] public Vector3 forward = Vector3.zero;

        private List<Vector3> pastPositions = new List<Vector3>();

        private int pastPositionsMaxTick = 30;

        private int lastHitByPlayerID = -1;

        private float JumpVal = 0;

        [Header("Animation Info")]

        public CombatAnimations LeftClickAnimation;

        public CombatAnimations RightClickAnimation;

        public void Initialize(int _id, string _username, Vector3 _spawnPosition)
        {
            id = _id;
            username = _username;
            transform.position = SpawnPoints.GetSpawnPoint(teamdex).position;
            inputs = new bool[8];
            LeftClickCooldown = new TimedCallback(EnableLeftMouseClick, 100f,LeftMouseDown);
            RightClickCooldown = new TimedCallback(EnableRightMouseClick, 100f,RightMouseDown);
            name = "Player: " + id;

            InitVar();
        }

        private void InitVar()
        {
            SetMass(120);
            health = maxHealth;
            score = 0;
            deaths = 0;

            tag = "Player";
        }

        public override void OnReset()
        {
            try
            {
                MoveToSpawn();
            }
            catch (Exception e)
            {
                Debug.Log($"Cant Move {e}");
            }
            

            health = maxHealth;
            deaths += 1;
            if (lastHitByPlayerID > 0)
            {
                Server.clients[lastHitByPlayerID].player.score += 1;
                ServerSend.UpdateScore(Server.clients[lastHitByPlayerID].player);
            }
            ServerSend.UpdateScore(this);
            ServerSend.UpdateHealth(this);
            transform.LookAt(Vector3.zero);
        }

        private void MoveToSpawn()
        {
            Vector3 temp = SpawnPoints.GetSpawnPoint(teamdex).position;
            //Debug.Log($"Moving to {temp}");
            cc.enabled = false;
            transform.position = temp;
            cc.enabled = true;
        }

        #region Basic Functions

        private void Update()
        {
            //Debug.Log($"Grounded state = {cc.isGrounded}");

            if (inputs[4] != true && isGrounded)
                canJump = true;

            Vector3 _inputDir = Vector3.zero;
            _inputDir.y += (inputs[0]?1:0) - (inputs[1]?1:0);
            _inputDir.x += (inputs[2] ? 1 : 0) - (inputs[3] ? 1 : 0);
            _inputDir.z += (inputs[4]? 1 : 0);

            if (inputs[5])
            {
                moveSpeed = baseMoveSpeed * runModifer;
            } else
            {
                moveSpeed = baseMoveSpeed;
            }

            Animation(_inputDir);

            Move(_inputDir);

            MouseClick(inputs[6], LeftClickCooldown, ref LeftClickExit);
            MouseClick(inputs[7], RightClickCooldown, ref RightClickExit);
        }

        private void MouseClick(bool clickState, TimedCallback projectileTimer, ref bool clickExit)
        {
            if (!clickExit && !clickState)
            {
                projectileTimer.flexbool = true;
            }

            if (clickState && !clickExit && projectileTimer.flexbool)
            {
                //CreateItem(inputTick); //OLD PROJECTILE
                clickExit = true;
                projectileTimer.Start();
            }
            else
            {
                projectileTimer.Update();
            }
        }

        private void Animation(Vector3 _inputDir) 
        {
            if (!isGrounded)
            {
                animationState = (int)AnimationStates.falling;
            } else
            {
                animationState = (int)AnimationStates.idle;
                if (inputs[5])
                {
                    animationState = (int)AnimationStates.running;
                }
            }
            if (Attacking)
            {
                animationState = (int)AnimationStates.swingSword;
            }
            ServerSend.playerAnimation(this, _inputDir.x, _inputDir.y);
            return;

        }

        //MAIN
        private void Move(Vector3 _inputDir)
        {
            forward = transform.forward;
            Vector3 _right = Vector3.Normalize(Vector3.Cross(forward, new Vector3(0, 1, 0)));

            Vector3 _moveDir = _right * _inputDir.x + forward * _inputDir.y;
            if (_moveDir != Vector3.zero) _moveDir = Vector3.Normalize(_moveDir);

            moveVector = _moveDir * moveSpeed * UnityEngine.Time.deltaTime;

            cc.Move(moveVector);

            JumpVal = _inputDir.z;

            ServerSend.PlayerRotation(this);
        }

        public void SetInput(bool[] _inputs)
        {
            inputs = _inputs;
        }

        private void SavePosition()
        {
            pastPositions.Insert(0, transform.position);
            if(pastPositions.Count > pastPositionsMaxTick)
            {
                pastPositions.RemoveAt(pastPositions.Count - 1);
            }
        }

        public bool TakeDamage(float amount, int projectileOwnerID)
        {
            if (projectileOwnerID == id || owner == projectileOwnerID)
                return false;

            lastHitByPlayerID = projectileOwnerID;

            health -= amount;

            if (lastHitByPlayerID >= 0)
                ServerSend.HitDamageObject(lastHitByPlayerID,health <= 0);

            if (health <= 0)
            {
                health = maxHealth;
                ResetObject();
                return true;
            } 
            ServerSend.UpdateHealth(this);
            return true;
        }

        #endregion

        #region Physics Functions

        private void FixedUpdate()
        {
            PhyisicsUpdate(JumpVal);
            Phyisics();
            ServerSend.PlayerPosition(this);
        }

        public void PhyisicsUpdate(float jump)
        {
            if (isGrounded)
            {
                if (canJump)
                {
                    veclocity.y = 0;
                    veclocity.y += jump * jumpHeight;
                    isGrounded = false;
                }
            }
            else
            {
                canJump = false;
                if (jump == 1 && veclocity.y >= 0)
                {
                    ApplyGravity(-.5f);
                }
            }
            //SavePosition();
        }

        #region Old Colide
        /*private void OnTriggerStay(Collider colider)
        {

            #region oldTrigger
            *//*PhysicsObject physObject = colider.GetComponent<PhysicsObject>();

            if (physObject == null)
                return;

            if (physObject.id == id)
                    return;
                if (physObject.owner == id)
                    return;

                if (physObject.createdTick >= pastPositions.Count)
                    physObject.createdTick = pastPositions.Count;

                if (physObject.tag == "Fireball")
                {
                    *//*if (hit.Length() <= 1.5f)
                    {*//*
                    TakeDamage();

                    Vector3 tmp = pastPositions[physObject.createdTick - 1] + new Vector3(0, 1, 0);

                    lastHitByPlayerID = physObject.owner;

                    tmp.y = physObject.transform.position.y;
                    *//*hit = tmp - ItemPos;

                    Vector3 hitNormal = hit / hit.Length();*//*
                    float mP = GetMass();
                    float mI = physObject.GetMass();

                    Vector3 tempVel = physObject.veclocity;

                    *//*item.veclocity = (
                        ((mP - mI) / (mI + mP) * (tempVel)) +
                        (2 * mI) / (mI + mP) * (veclocity)
                        ).Length() * -hitNormal;*//*

                    veclocity = (
                        ((2 * mI) / (mI + mP) * (tempVel)) +
                        (mP - mI) / (mI + mP) * (veclocity)
                        ).magnitude * Server.clients[physObject.owner].player.forward; //hitNormal;

                        Destroy(physObject.gameObject); 
                    *//*}*//*
                }
                    

                if (physObject.tag == "Player")
                {
                    transform.position -= moveVector;
                }*//*
            #endregion

        }//end Colide*/
        #endregion

        #endregion

        #region Action Functions

        private void EnableRightMouseClick()
        {
            RightClickExit = false;
        }

        private void EnableLeftMouseClick()
        {
            LeftClickExit = false;
        }

        private void LeftMouseDown()
        {
            CreateRayProjectile(0);
        }

        private void RightMouseDown()
        {
            CreateRayProjectile(1);
        }

        private void CreateItem(int _inputTick)
        {
/*            for (int i = 1; i < Server.MaxItems; i++)
            {
                if (Server.items[i].created == false)
                {*/ 
                    Projectile projectile = Prefabs.instance.InstantiatePrefab<Projectile>(Prefabs.instance.Item);
                        projectile?.Initialize(transform.rotation, transform.position + new Vector3(0,1.5f,0), id, _inputTick);
                    return;
/*                }
            }*/
        }

        private void CreateRayProjectile(int upid) //PROJECTILE INDEX 0
        {
            Vector3 hitpoint = ShootLocation.position + Head.forward * 30f;
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(ShootLocation.position, Head.forward, out hit, Mathf.Infinity))
            {
                hitpoint = hit.point;
                Damageable isDamageable = hit.collider.GetComponent<Damageable>();
                if (isDamageable != null)
                {
                    isDamageable.TakeDamage(Damage,id);
                }
            }
            ServerSend.ProjectileHit(HandLocation.position, hitpoint, upid);
        }
        #endregion
    }
}
