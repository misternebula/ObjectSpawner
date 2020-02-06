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

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            solanum = Instantiate(GameObject.Find("Character_NOM_Solanum"));
            solanum.SetActive(false);
            StreamingManager.LoadStreamingAssets("quantummoon/meshes/characters");

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
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                PlaceObjectRaycast(Instantiate(solanum));
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                PlaceObjectRaycast(Instantiate(esker));
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                PlaceObjectRaycast(Instantiate(campfire));
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                PlaceObjectRaycast(Instantiate(tektite));
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                PlaceObjectRaycast(Instantiate(reibeck));
            }
        }

        void PlaceObject(Vector3 normal, Vector3 point, GameObject gameObject, OWRigidbody targetRigidbody)
        {
            Transform parent = targetRigidbody.transform;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(parent);
            Quaternion lhs = Quaternion.FromToRotation(gameObject.transform.TransformDirection(Vector3.up), normal);
            gameObject.transform.rotation = lhs * gameObject.transform.rotation;
            gameObject.transform.position = point + gameObject.transform.TransformDirection(Vector3.zero);
        }

        void PlaceObjectRaycast(GameObject gameObject)
        {
            if (IsPlaceable(out Vector3 placeNormal, out Vector3 placePoint, out OWRigidbody targetRigidbody))
            {
                PlaceObject(placeNormal, placePoint, gameObject, targetRigidbody);
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
