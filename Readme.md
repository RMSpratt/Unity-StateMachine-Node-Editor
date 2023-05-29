# StateMachine Node Editor
This repository holds the template code for creating StateMachines for AI-controlled
characters.

The StateMachine components are based on a modified version of the StateMachine
seen here: https://github.com/RMSpratt/Unity-AI-Templates/tree/main/StateMachine.

This Node editor hopes to provide a more designer-friendly method for designing complex
state machine behaviour for one or more game characters, (Agents).

The StateMachine behaviour as described relies on **Blackboard** entities.


## Organization
Four sets of classes are used for converting designer-data defined in the UnityEditor
to actionable behaviour for StateMachine agents at runtime.

![High-Level-Entity-View](./Images/Planning/Design-Build-Run-Entities.drawio.png?raw=true "High-Level View")

The subsequent sections of this ReadMe will explore each set of entities as described
in the high-level overview.

### Editor
StateMachine building for agents begins in the UnityEditor with a custom EditorWindow.

Editor data is tied to a single **StateMachineGraph** as a ScriptableObject maintaining all
serialized StateMachine design information for an Agent.

Each StateMachine component: States, Transitions, and Conditions can be created and modified
through this window. Some running screenshots are supplied [here](./Images/Unity/NodeEditor).

### Serialization
The Editor is populated with data taken from serialized classes for representing States,
Transitions, and Conditions. All three types of classes are non-MonoBehaviour class instances 
maintained by the **StateMachineGraph**.

Changes made through the Editor are applyed to serialized representations of these class instances.

### Builders
The set of classes known as "Builders" are used to bridge the gap between serialized designer data
and the run-time StateMachine code.

Each builder for States, Transitions, and Conditions acts as a Factory for converting serialized class
representations into the expected versions for a run-time StateMachine.

### StateMachine
The StateMachine itself only exists at runtime and is created and accessed through a **StateMachineAgent**
MonoBehaviour component script.

This allows for specific Agent scripts and functionality to remain separate from the StateMachine itself.

#### Components
As has been mentioned in the descriptions above, StateMachines for controlling the AI behaviour of an agent
is composed of :

1. A set of States with actions to be carried out. These can be defined as entry-state actions, in-state actions,
or exit-state actions.

2. A set of Transitions for navigating between States. These can also be defined with actions to carry out.

3. A set of Conditions defining when an Agent should transition between States. These evaluate to true or false
when evaluated.


## StateMachine Agents
GameObjects can become Agents capable of using the StateMachine functionality by accessing the **StateMachineAgent**
component script. 

This Component script maintains three things for defining agent behaviour:

- A StateMachineGraph
- A Blackboard
- A set of ActionInfo instances

### Blackboards
--TO_DO--

### ActionInfo
ActionInfo classes are used to maintain designer-friendly actions that can be assigned to States and Transitions
in the StateMachineGraph editor window.

These essentially map a single string (action name) to one or more functions to invoke as part of a UnityEvent.

The list of ActionInfo classes appear as options when selecting actions to add to Editor StateNodes and TransitionLinks.
At runtime, Builder classes will examine each ActionInfo's UnityEvent and aggregate its listeners into a single UnityEvent
to be invoked for a State or TransitionLink.

## To-Do Features
A list of features to be added including QoL fixes and new functionality.

**Editor**
- Select StateTransition by clicking on links.
- Support for ScriptableObjects in Condition Static Values

- Support for StateMachine any-state transitions
- Support for StateMachine layer entry actions
- Support for StateMachine layer exit actions

**Agent**
- AgentModel ScriptableObject for static designer-data

**Overall**
- Editor Support for Hierarchical State Machines

