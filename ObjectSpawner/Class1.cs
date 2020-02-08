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
        public GameObject solanum;
        public GameObject esker;
        public GameObject campfire;
        public GameObject tektite;
        public GameObject reibeck;
        public GameObject mica;
        public GameObject NOMstaff;

        public GameObject edit;

        public bool moveSpeedFast = false;
        public float moveSpeed = 0.1f;
        public bool editMode = false;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Create copies of any object we want to spawn. This way we can
            // edit their properties here, instead of editing them in the spawn
            // function.
            solanum = Instantiate(GameObject.Find("Character_NOM_Solanum"));
            solanum.SetActive(false);

            mica = Instantiate(GameObject.Find("Villager_HEA_Mica"));
            mica.SetActive(false);

            NOMstaff = Instantiate(GameObject.Find("Prefab_NOM_Staff"));
            NOMstaff.SetActive(false);

            /*
            esker = Instantiate(GameObject.Find("Villager_HEA_Esker"));
            esker.SetActive(false);
            StreamingManager.LoadStreamingAssets("timberhearth/meshes/characters");

            campfire = Instantiate(GameObject.Find("Effects_HEA_Campfire"));
            campfire.SetActive(false);

            tektite = Instantiate(GameObject.Find("Villager_HEA_Tektite_2"));
            tektite.SetActive(false);

            reibeck = Instantiate(GameObject.Find("Traveller_HEA_Riebeck"));
            reibeck.SetActive(false);
            StreamingManager.LoadStreamingAssets("brittlehollow/meshes/characters");
            */
        }

        void Update()
        {
            // Toggle edit mode
            if (Input.GetKeyDown(KeyCode.KeypadDivide))
            {
                editMode = !editMode;
                NotificationData data = editMode ? new NotificationData(NotificationTarget.Player, "ENTERING EDIT MODE", 3f, true) : new NotificationData(NotificationTarget.Player, "EXITING EDIT MODE", 3f, true);
                NotificationManager.SharedInstance.PostNotification(data, false);
            }

            if (editMode)
            {
                if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                {
                    Vector3 forward = Locator.GetPlayerCamera().transform.forward;
                    RaycastHit hit;

                    if (Physics.Raycast(Locator.GetPlayerCamera().transform.position, forward, out hit, 100f, OWLayerMask.physicalMask | OWLayerMask.interactMask))
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
            else
            {
                if (Input.GetKeyDown(KeyCode.Keypad0))
                {
                    PlaceObjectRaycast(Instantiate(solanum), true);
                    StreamingManager.LoadStreamingAssets("quantummoon/meshes/characters");
                }

                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    PlaceObjectRaycast(Instantiate(mica), true);
                }

                if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    PlaceObjectRaycast(Instantiate(NOMstaff), true);
                    StreamingManager.LoadStreamingAssets("brittlehollow/meshes/props");
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
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "EditModeSphere";
            sphere.transform.parent = edit.transform;
            sphere.transform.localPosition = Vector3.zero;
            sphere.DestroyAllComponents<Collider>();

            // Set colour and transparency
            sphere.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            sphere.GetComponent<MeshRenderer>().material.SetAlpha(0.5f);
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
    }
}
