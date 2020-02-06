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

        void PlaceObjectRaycast(GameObject gameObject)
        {
            if (IsPlaceable(out RaycastHit hit, out OWRigidbody targetRigidbody))
            {
                Transform parent = targetRigidbody.transform;
                gameObject.SetActive(true);
                gameObject.transform.SetParent(parent);
                Quaternion lhs = Quaternion.FromToRotation(gameObject.transform.TransformDirection(Vector3.up), hit.normal);
                gameObject.transform.rotation = lhs * gameObject.transform.rotation;
                gameObject.transform.position = hit.point + gameObject.transform.TransformDirection(Vector3.zero);
            }
        }

        bool IsPlaceable(out RaycastHit hit, out OWRigidbody targetRigidbody)
        {
            hit = default(RaycastHit);
            targetRigidbody = null;

            Vector3 forward = Locator.GetPlayerTransform().forward;
            Vector3 forward2 = Locator.GetPlayerCamera().transform.forward;
            
            if (Physics.Raycast(Locator.GetPlayerCamera().transform.position, forward2, out hit, 100f, OWLayerMask.physicalMask | OWLayerMask.interactMask))
            {
                targetRigidbody = hit.collider.GetAttachedOWRigidbody(false);
                return true;
            }
            return false;
        }
    }
}
