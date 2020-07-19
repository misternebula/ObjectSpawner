using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ObjectSpawner
{
    public class MainClass : ModBehaviour
    {
        public List<ObjectBase> objectList = new List<ObjectBase>();

        public GameObject edit;

        public bool moveSpeedFast = false;
        public float moveSpeed = 0.1f;
        public bool editMode = false;

        public AssetBundle _assetBundle;

        public static IModHelper helper;

        public static bool inputEnabled = false;
        private void Start()
        {
            base.ModHelper.Console.WriteLine("[ObjectSpawner] :");
            _assetBundle = base.ModHelper.Assets.LoadBundle("selectionbundle");
            helper = base.ModHelper;
            if (_assetBundle != null)
            {
                base.ModHelper.Console.WriteLine(":     Assetbundle loaded successfully.");
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Create copies of any object we want to spawn. This way we can
            // edit their properties here, instead of editing them in the spawn
            // function.
            if (LoadManager.GetCurrentScene() == OWScene.SolarSystem || LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse)
            {

                base.ModHelper.Console.WriteLine(":     Destroying old GOs...");

                foreach (var item in GameObject.FindObjectsOfType<BeepBoop>())
                {
                    GameObject.Destroy(item.gameObject);
                }

                base.ModHelper.Console.WriteLine(":     ObjectSpawner GO load...");

                //objectList.Add(new SolanumObject("Nomai_ANIM_SkyWatching_Idle", "quantummoon/meshes/characters"));

                if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
                {
                    objectList.Add(new ObjectBase("Nomai_ANIM_SkyWatching_Idle", "quantummoon/meshes/characters"));

                    objectList.Add(new ObjectBase("Villager_HEA_Mica", "timberhearth/meshes/characters"));

                    objectList.Add(new ObjectBase("Prefab_NOM_Staff", "brittlehollow/meshes/props"));

                    objectList.Add(new ObjectBase("Prefab_NOM_Torch", ""));

                    objectList.Add(new ObjectBase("EscapePodFlare_Body", "darkbramble/meshes/props"));

                    objectList.Add(new SelfObject("NPC_Player", ""));

                    objectList.Add(new SelfObject("Villager_HEA_Gossan", "timberhearth/meshes/characters"));

                    objectList.Add(new SelfObject("Villager_HEA_Hornfels (1)", "timberhearth/meshes/characters"));

                    objectList.Add(new SelfObject("Villager_HEA_Hal_Museum", "timberhearth/meshes/characters"));
                }

                base.ModHelper.Console.WriteLine(":     Successfully loaded [" + objectList.Count + "] object(s).");
            }
        }

        void Update()
        {
            if (inputEnabled)
            {
                // Toggle edit mode
                if (Input.GetKeyDown(KeyCode.KeypadDivide))
                {
                    editMode = !editMode;
                    NotificationData data = editMode ? new NotificationData(NotificationTarget.Player, "ENTERING EDIT MODE", 3f, true) : new NotificationData(NotificationTarget.Player, "EXITING EDIT MODE", 3f, true);
                    NotificationManager.SharedInstance.PostNotification(data, false);
                    if (!editMode)
                    {
                        Destroy(GameObject.Find("EditModeSphere"));
                    }
                }

                if (editMode)
                {
                    if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                    {
                        Vector3 forward = Locator.GetPlayerCamera().transform.forward;

                        if (Physics.Raycast(Locator.GetPlayerCamera().transform.position, forward, out RaycastHit hit, 100f, OWLayerMask.physicalMask | OWLayerMask.interactMask))
                        {
                            edit = hit.collider.gameObject;
                            UpdateEditHud();
                            UpdateEditUI();
                        }
                    }

                    // Toggle fast/slow movement
                    if (Input.GetKeyDown(KeyCode.ScrollLock))
                    {
                        moveSpeedFast = !moveSpeedFast;
                        moveSpeed = moveSpeedFast ? 10f : 0.1f;
                        NotificationData data = moveSpeedFast ? new NotificationData(NotificationTarget.Player, "FAST MOVEMENT", 3f, true) : new NotificationData(NotificationTarget.Player, "SLOW MOVEMENT", 3f, true);
                        NotificationManager.SharedInstance.PostNotification(data, false);
                    }

                    // Delete selected object
                    if (Input.GetKeyDown(KeyCode.KeypadMinus))
                    {
                        NotificationData data = new NotificationData(NotificationTarget.Player, "DELETING " + edit.name + " FROM SCENE", 3f, true);
                        NotificationManager.SharedInstance.PostNotification(data, false);
                        Destroy(edit);
                    }

                    // Select parent
                    if (Input.GetKeyDown(KeyCode.KeypadPeriod))
                    {
                        edit = edit.transform.parent.gameObject;
                        UpdateEditHud();
                        UpdateEditUI();
                    }

                    // ROTATION //
                    if (Input.GetKeyDown(KeyCode.Keypad7))
                    {
                        edit.transform.Rotate(0, 10, 0);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad9))
                    {
                        edit.transform.Rotate(0, -10, 0);
                    }

                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        edit.transform.Rotate(-10, 0, 0);
                    }

                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        edit.transform.Rotate(10, 0, 0);
                    }

                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        edit.transform.Rotate(0, 0, -10);
                    }

                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        edit.transform.Rotate(0, 0, 10);
                    }

                    // MOVEMENT //
                    if (Input.GetKeyDown(KeyCode.Keypad4))
                    {
                        edit.transform.Translate(moveSpeed, 0f, 0f);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad6))
                    {
                        edit.transform.Translate(-moveSpeed, 0f, 0f);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad2))
                    {
                        edit.transform.Translate(0f, 0f, moveSpeed);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad8))
                    {
                        edit.transform.Translate(0f, 0f, -moveSpeed);
                    }

                    if (Input.GetKeyDown(KeyCode.KeypadPlus))
                    {
                        edit.transform.Translate(0f, moveSpeed, 0f);
                    }

                    if (Input.GetKeyDown(KeyCode.KeypadEnter))
                    {
                        edit.transform.Translate(0f, -moveSpeed, 0f);
                    }
                }
                else // Object Spawning Mode
                {
                    if (Input.GetKeyDown(KeyCode.Keypad0))
                    {
                        PlaceObjectRaycast(objectList[0].Load(), true);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad1))
                    {
                        PlaceObjectRaycast(objectList[1].Load(), true);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad2))
                    {
                        PlaceObjectRaycast(objectList[2].Load(), true);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad3))
                    {
                        PlaceObjectRaycast(objectList[3].Load(), true);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad4))
                    {
                        PlaceObjectRaycast(objectList[4].Load(), true);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad5))
                    {
                        PlaceObjectRaycast(objectList[5].Load(), true);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad6))
                    {
                        PlaceObjectRaycast(objectList[6].Load(), true);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad7))
                    {
                        PlaceObjectRaycast(objectList[7].Load(), true);
                    }

                    if (Input.GetKeyDown(KeyCode.Keypad8))
                    {
                        PlaceObjectRaycast(objectList[8].Load(), true);
                    }
                }
            }
        }

        void UpdateEditHud()
        {
            NotificationData selectedObject = new NotificationData(NotificationTarget.Player, "EDITING : " + edit.name, 3f, true);
            NotificationManager.SharedInstance.PostNotification(selectedObject, false);
        }

        void UpdateEditUI()
        {
            // Destroy old sphere and create new one
            Destroy(GameObject.Find("EditModeSphere"));
            GameObject sphere = Instantiate(_assetBundle.LoadAsset<GameObject>("sphere"));
            sphere.name = "EditModeSphere";
            sphere.transform.parent = edit.transform;
            sphere.transform.localPosition = Vector3.zero;
            sphere.DestroyAllComponents<SphereCollider>();

            sphere.transform.localScale = edit.GetComponent<Collider>().bounds.size;
            if (edit.GetComponent<SkinnedMeshRenderer>() != null)
            {
                sphere.transform.localScale = edit.GetComponent<SkinnedMeshRenderer>().bounds.size;
            }
        }

        void PlaceObject(Vector3 normal, Vector3 point, GameObject gameObject, OWRigidbody targetRigidbody, bool lookAtPlayer)
        {
            // Get object to parent gameObject to
            Transform parent = targetRigidbody.transform;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(parent);
            gameObject.transform.position = point + gameObject.transform.TransformDirection(Vector3.zero);

            if (lookAtPlayer)
            {
                // Thanks Rai!
                var look = Vector3.ProjectOnPlane(Locator.GetPlayerTransform().transform.position - gameObject.transform.position, normal);
                gameObject.transform.rotation = Quaternion.LookRotation(look, normal);
            }

            if (gameObject.GetComponentInChildren<OWCollider>() != null)
            {
                gameObject.GetComponentInChildren<OWCollider>().SetActivation(true);
                gameObject.GetComponentInChildren<OWCollider>().enabled = true;
            }

            gameObject.AddComponent<BeepBoop>();
        }

        void PlaceObjectRaycast(GameObject gameObject, bool lookAtPlayer = false)
        {
            if (IsPlaceable(out Vector3 placeNormal, out Vector3 placePoint, out OWRigidbody targetRigidbody))
            {
                PlaceObject(placeNormal, placePoint, gameObject, targetRigidbody, lookAtPlayer);
            }
        }

        bool IsPlaceable(out Vector3 placeNormal, out Vector3 placePoint, out OWRigidbody targetRigidbody)
        {
            placeNormal = Vector3.zero;
            placePoint = Vector3.zero;
            targetRigidbody = null;

            Vector3 forward = Locator.GetPlayerCamera().transform.forward;

            if (Physics.Raycast(Locator.GetPlayerCamera().transform.position, forward, out RaycastHit hit, 100f, OWLayerMask.physicalMask | OWLayerMask.interactMask))
            {
                placeNormal = hit.normal;
                placePoint = hit.point;
                targetRigidbody = hit.collider.GetAttachedOWRigidbody(false);
                return true;
            }
            return false;
        }

        public static void MNActivateInput()
        {
            inputEnabled = true;
        }

        public static void MNDeactivateInput()
        {
            inputEnabled = false;
        }
    }
}
