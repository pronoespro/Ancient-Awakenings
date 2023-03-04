using Ancient_Awakenings_SoulNail_charm.Monobehaviors;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Ancient_Awakenings_SoulNail_charm
{
    public class Ancient_Awakenings_SoulNail_charm : Mod,ITogglableMod
    {
        #region Instancing
        public Ancient_Awakenings_SoulNail_charm() : base("Ancien Awakenings Soulnail Charm") { }

        internal static Ancient_Awakenings_SoulNail_charm Instance;

        public override string GetVersion()
        {
            return "v1.0.0.0";
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            LoadBundles();

            StartSoulNailCharm();

            LoadUI();

            //On.HeroController.Update += HeroController_Update;

            Log("Initialized");
        }

        public void Unload()
        {
            //On.HeroController.Update -= HeroController_Update;
            UnloadSoulNailCharm();
            UnloadBundles();
            UnloadUI();

            Instance = null;
        }
        #endregion

        #region bundles

        public Dictionary<string, AssetBundle> AttackBundles;
        private List<string> attacksToLoad = new List<string> { "charmattacks" };

        public Dictionary<string, AssetBundle> UIBundles;
        private List<string> uiToLoad = new List<string> { "ui" };

        private void LoadBundles()
        {

            AttackBundles = new Dictionary<string, AssetBundle>();
            UIBundles = new Dictionary<string, AssetBundle>();


            Assembly asm = Assembly.GetExecutingAssembly();
            Log("Searching for Levels");
            foreach (string res in asm.GetManifestResourceNames())
            {
                using (Stream s = asm.GetManifestResourceStream(res))
                {
                    if (s == null)
                    {
                        continue;
                    }
                    Log("Found asset");

                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();
                    string bundleName = Path.GetExtension(res).Substring(1);


                    if (attacksToLoad.Contains(bundleName))
                    {
                        Log("Found attack " + bundleName);
                        AttackBundles.Add(bundleName, AssetBundle.LoadFromMemory(buffer));
                    }else if (uiToLoad.Contains(bundleName))
                    {
                        Log("Found attack " + bundleName);
                        UIBundles.Add(bundleName, AssetBundle.LoadFromMemory(buffer));
                    }
                    else
                    {
                        continue;

                    }

                }
            }
        }

        private void UnloadBundles()
        {
            AttackBundles = null;
            UIBundles = null;
        }

        #endregion

        #region UI

        public Canvas uiPanel;

        private void LoadUI()
        {
            if (UIBundles.ContainsKey("ui"))
            {
                AssetBundle _bundle = UIBundles["ui"];
                if(_bundle!=null)
                {
                    uiPanel = GameObject.Instantiate(_bundle.LoadAsset<GameObject>("Canvas")).GetComponent<Canvas>();
                    Log("Loaded Canvas");

                    uiPanel.gameObject.AddComponent<AncientUI>();

                    GameObject.DontDestroyOnLoad(uiPanel);
                }
            }
        }

        private void UnloadUI(){
            if (uiPanel != null){
                GameObject.Destroy(uiPanel);
            }
        }

        #endregion

        #region powerups

        //Soul Nail Charm
        private int soulNailHitCount;
        private int maxSoulHitCount=5;
        private GameObject soulNailProjectile;

        public bool radianceDeffeated;
        private HitDataList hitDatas;
        private float refreshHitTimer = 0.5f;
        private string[] invincibleEnemies = new string[] { "Zombie Beam Miner" };
        
        public Vector2 GetSoulNailHit()
        {
            return new Vector2(soulNailHitCount, maxSoulHitCount);
        }

        public void StartSoulNailCharm()
        {
            On.HealthManager.Hit += HealthManager_Hit;
            On.HealthManager.Update += HealthManager_Update;
        }

        private void UnloadSoulNailCharm()
        {
            On.HealthManager.Hit -= HealthManager_Hit;
            On.HealthManager.Update -= HealthManager_Update;
        }

        private void HealthManager_Update(On.HealthManager.orig_Update orig, HealthManager self)
        {
            if (hitDatas != null) {
                hitDatas.Update();
            }
            orig(self);
        }

        private bool IsInvincibleEnemy(string name)
        {

            foreach(string s in invincibleEnemies)
            {
                if (name.Contains(s))
                {
                    return true;
                }
            }

            return false;
        }

        private void HealthManager_Hit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitDatas == null){
                hitDatas = new HitDataList();
            }

            if (self!=HeroController.instance &&
                PlayerData.instance.GetBool(nameof(PlayerData.instance.equippedCharm_15)) 
                && hitInstance.AttackType == AttackTypes.Nail && !hitDatas.Contains(hitInstance.Source.transform,self.transform)
                && (!self.IsInvincible || IsInvincibleEnemy(self.gameObject.name)))
            {

                hitDatas.Add(new HitData(hitInstance.Source.transform, self.transform,refreshHitTimer));

                soulNailHitCount++;
                if (soulNailHitCount % maxSoulHitCount == maxSoulHitCount-1)
                {
                    CreateSoulNailProj(self.gameObject.transform.position);
                }
            }

            orig(self,hitInstance);
        }

        private void CreateSoulNailProj(Vector3 pos)
        {
            if (soulNailProjectile == null)
            {
                if (AttackBundles.ContainsKey("charmattacks"))
                {
                    AssetBundle atk = AttackBundles["charmattacks"];
                    if (atk != null && atk.Contains("SoulNailProjectile"))
                    {
                        soulNailProjectile = GameObject.Instantiate(atk.LoadAsset<GameObject>("SoulNailProjectile"),pos,Quaternion.identity);
                        SoulNail_proj nailProj= soulNailProjectile.AddComponent<SoulNail_proj>();
                        Transform child= soulNailProjectile.transform.GetChild(0).GetChild(1);

                        ProjCollision col = child.gameObject.AddComponent<ProjCollision>();
                        col.proj = nailProj;

                        col.gameObject.AddComponent<NonBouncer>();

                        DamageEnemies dmg = child.gameObject.AddComponent<DamageEnemies>();

                        dmg.attackType = AttackTypes.Spell;
                        dmg.damageDealt = PlayerData.instance.nailDamage;

                        Log("Created Soul Nail Projectile");
                    }
                    else
                    {
                        Log("Attack not found");
                    }
                }else{
                    Log("Bundle not found");
                }
            }
            else
            {
                soulNailProjectile.SetActive(true);
                soulNailProjectile.transform.position = pos;
                soulNailProjectile.GetComponent<SoulNail_proj>().Restart();

                Log("Created Soul Nail Projectile");
            }
        }

        #endregion

        #region Uninfected Crossroads
        /*
        private void HeroController_Update(On.HeroController.orig_Update orig, HeroController self)
        {
            radianceDeffeated = PlayerData.instance.GetBool(nameof(PlayerData.instance.killedFinalBoss));

            if (radianceDeffeated)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.instance.crossroadsInfected), false);
                PlayerData.instance.crossroadsInfected = false;
            }

            orig(self);
        }*/
        #endregion
    }
}