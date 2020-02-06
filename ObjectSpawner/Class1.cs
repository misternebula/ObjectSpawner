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

        public bool editMode = false;

        public List<GameObject> goList = new List<GameObject>();

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
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
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                NotificationData data = new NotificationData(NotificationTarget.Player, "DELETING " + goList[goList.Count-1].name + " FROM SCENE", 3f, true);
                NotificationManager.SharedInstance.PostNotification(data, false);
                Destroy(goList[goList.Count-1]);
                goList.Remove(goList[goList.Count-1]);
            }

            if (Input.GetKeyDown(KeyCode.KeypadDivide))
            {
                NotificationData enterEdit = new NotificationData(NotificationTarget.Player, "ENTERING EDIT MODE", 3f, true);
                NotificationData exitEdit = new NotificationData(NotificationTarget.Player, "EXITING EDIT MODE", 3f, true);
                if (!editMode)
                {
                    editMode = true;
                    NotificationManager.SharedInstance.PostNotification(enterEdit, false);
                }
                else
                {
                    editMode = false;
                    NotificationManager.SharedInstance.PostNotification(exitEdit, false);
                }
            }

            if (editMode)
            {
                if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                {
                    Vector3 forward = Locator.GetPlayerTransform().forward;
                    Vector3 forward2 = Locator.GetPlayerCamera().transform.forward;
                    RaycastHit hit;

                    if (Physics.Raycast(Locator.GetPlayerCamera().transform.position, forward2, out hit, 100f, OWLayerMask.physicalMask | OWLayerMask.interactMask))
                    {
                        edit = hit.collider.gameObject;
                        UpdateEditHud();
                        UpdateEditUI();
                    }
                }

                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    edit = edit.transform.parent.gameObject;
                    UpdateEditHud();
                    UpdateEditUI();
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
            sphere.transform.parent = edit.transform;
            sphere.transform.localPosition = Vector3.zero;
        }

        void PlaceObject(Vector3 normal, Vector3 point, GameObject gameObject, OWRigidbody targetRigidbody, bool lookAtPlayer)
        {
            Transform parent = targetRigidbody.transform;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(parent);
            Quaternion lhs = Quaternion.FromToRotation(gameObject.transform.TransformDirection(Vector3.up), normal);
            gameObject.transform.rotation = lhs * gameObject.transform.rotation;
            gameObject.transform.position = point + gameObject.transform.TransformDirection(Vector3.zero);
            if (lookAtPlayer)
            {
                base.ModHelper.Console.WriteLine("GO is : " + gameObject.transform.forward);
                base.ModHelper.Console.WriteLine("Player is : " + Locator.GetPlayerTransform().forward);
                //gameObject.transform.RotateAround(point, normal, Vector3.Angle(gameObject.transform.forward, Locator.GetPlayerTransform().forward) + 180);
                gameObject.transform.LookAt(new Vector3(gameObject.transform.position.x, Locator.GetPlayerTransform().position.y, gameObject.transform.position.z));
            }
        }

        void PlaceObjectRaycast(GameObject gameObject, bool lookAtPlayer = false)
        {
            if (IsPlaceable(out Vector3 placeNormal, out Vector3 placePoint, out OWRigidbody targetRigidbody))
            {
                goList.Add(gameObject);
                PlaceObject(placeNormal, placePoint, gameObject, targetRigidbody, lookAtPlayer);
            }
        }

        bool IsPlaceable(out Vector3 placeNormal, out Vector3 placePoint, out OWRigidbody targetRigidbody)
        {
            placeNormal = Vector3.zero;
            placePoint = Vector3.zero;
            targetRigidbody = null;

            Vector3 forward = Locator.GetPlayerTransform().forward;
            Vector3 forward2 = Locator.GetPlayerCamera().transform.forward;

            RaycastHit hit;
            
            if (Physics.Raycast(Locator.GetPlayerCamera().transform.position, forward2, out hit, 100f, OWLayerMask.physicalMask | OWLayerMask.interactMask))
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
