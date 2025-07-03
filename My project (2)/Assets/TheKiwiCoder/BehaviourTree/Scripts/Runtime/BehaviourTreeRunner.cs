using AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class BehaviourTreeRunner : MonoBehaviour {

        // The main behaviour tree asset
        public BehaviourTree tree;

        // Storage container object to hold game object subsystems
        Context context;

        private AbilitySystemCharacter playerASC;

        public bool Stop = false;

        private void Awake()
        {
            //playerASC = GameObject.FindGameObjectWithTag("Player").GetComponent<AbilitySystemCharacter>();
        }

        // Start is called before the first frame update
        void Start() {
            context = CreateBehaviourTreeContext();
            tree = tree.Clone();
            tree.Bind(context);
            var playerASC = GameObject.FindGameObjectWithTag("Player").GetComponent<AbilitySystemCharacter>();
            tree.blackboard.UpdateData<AbilitySystemCharacter>("target", playerASC);
            //tree.blackboard.target = GameObject.FindGameObjectWithTag("Player").GetComponent<AbilitySystemCharacter>();
        }

        // Update is called once per frame
        void Update() {
            if (Stop) {
                return;
            }
            if (tree) {
                tree.Update();
            }
        }

        Context CreateBehaviourTreeContext() {
            return Context.CreateFromGameObject(gameObject);
        }

        private void OnDrawGizmosSelected() {
            if (!tree) {
                return;
            }

            BehaviourTree.Traverse(tree.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}