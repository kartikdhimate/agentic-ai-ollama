# 🤖 Agentic AI with Ollama

A **learning-focused repository** exploring how to build intelligent AI agents using [Microsoft's Agents Framework](https://github.com/microsoft/agent-framework) and local LLMs via [Ollama](https://ollama.ai).

---

## 📋 Prerequisites

Before you start, ensure you have:

- **Ollama** installed and running locally → [Installation Guide](https://github.com/ollama/ollama#installation)
- **.NET SDK 10.0 or later** → [Download & Install](https://dotnet.microsoft.com/en-us/download)
- **Basic C# knowledge** (helpful but not required)
- **Ollama model pulled** → Run: `ollama pull llama3.2:latest` and `ollama pull bge-m3:latest` (for RAG sections)

**Estimated setup time:** 15–20 minutes (mostly waiting for Ollama to pull models)

---

## 🎯 What You'll Learn

| Concept | What You Get |
|---------|-------------|
| **AI Agents** | Understand how agents think, plan, and act |
| **Tool Use** | Give agents the ability to call functions and interact with systems |
| **Memory & Context** | Build stateful agents that remember conversation history |
| **Workflows** | Orchestrate multi-step processes with agents |
| **RAG** | Combine agents with retrieval-augmented generation for knowledge |
| **Agent-to-Agent** | Design systems where multiple agents collaborate |
| **MCP** | Integrate with Model Context Protocol for extensibility |
| **UI Integration** | Build web frontends for agentic systems |

---

## 📁 Project Structure

```
agentic-ai-ollama/
├── Ollama/                          # Shared Ollama configuration & extensions
├── Section-1-Getting-Started/       # Basics: Hello agents, minimal setup
├── Section-2-Tool-Use/              # Function calling & agent capabilities
├── Section-3-Memory/                # Stateful agents & context management
├── Section-4-Workflows/             # Simple workflow orchestration
├── Section-5-Workflow-Patterns/     # Advanced patterns & agent composition
├── Section-6-Agentic-RAG/           # Retrieval-augmented generation examples
├── Section-7-A2A/                   # Agent-to-agent communication
├── Section-8-MCP/                   # Model Context Protocol integration
├── Section-9-AG-UI/                 # UI & dashboard for agents
└── README.md                        # This file
```

---

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/kartikdhimate/agentic-ai-ollama.git
cd agentic-ai-ollama
```

### 2. Verify Ollama is Running
```bash
ollama list
# Should show: llama3.2:latest and bge-m3:latest
```

### 3. Run Your First Example
```bash
cd Section-1-Getting-Started/HelloAgent
dotnet run
```

You should see the agent respond to: _"Why is the sky blue?"_

---

## 📚 Learning Sections Overview

Each section builds on the previous one and includes **fully runnable examples**. Pick a section and explore!

### **Section 1: Getting Started**
- **Focus:** Agent fundamentals
- **Learn:** What agents are, basic prompts, synchronous execution
- **Examples:** `HelloAgent`, `BasicAgentApp`, `MinimalAgent`, `StructuredOutput`
- **Time:** 15 min

### **Section 2: Tool Use**
- **Focus:** Giving agents capabilities
- **Learn:** Function calling, tool invocation, agent autonomy
- **Examples:** `FunctionCall`, `ApproveRequiredFunction`, `MinimalAgentWithTools`
- **Time:** 20 min

### **Section 3: Memory**
- **Focus:** Building stateful agents
- **Learn:** Context providers, session management, persistent state
- **Examples:** `CustomContextProvider`, `MockCosmosDb`
- **Time:** 25 min

### **Section 4: Workflows**
- **Focus:** Orchestrating agent actions
- **Learn:** Workflow patterns, multi-step processes, agent coordination
- **Examples:** `FirstWorkflow`, `AgentsWorkflow`
- **Time:** 20 min

### **Section 5: Workflow Patterns**
- **Focus:** Advanced orchestration
- **Learn:** Complex patterns, enterprise scenarios, multi-agent choreography
- **Examples:** `AgenticWorkflowPatterns`, `MinimalAgentWithWorkflows`
- **Time:** 30 min

### **Section 6: Agentic RAG**
- **Focus:** Retrieval-augmented generation
- **Learn:** Knowledge integration, vector stores, semantic search
- **Examples:** `BasicTextRAGExample`, `QdrantVectorStore`
- **Time:** 25 min

### **Section 7: Agent-to-Agent (A2A)**
- **Focus:** Multi-agent systems
- **Learn:** Agent communication, delegation, collaborative problem-solving
- **Examples:** `EnterpriseComplianceService`
- **Time:** 25 min

### **Section 8: Model Context Protocol (MCP)**
- **Focus:** Extensibility & interoperability
- **Learn:** MCP servers, standardized tool integration
- **Examples:** `LocalGitHubMcpExample`, `HostedMcpGovernanceExample`
- **Time:** 20 min

### **Section 9: Agentic UI**
- **Focus:** User-facing agent systems
- **Learn:** Web frontends, real-time updates, deployment patterns
- **Examples:** `AgUiServer`, `AgUiClient`
- **Time:** 30 min

---

## 🏃 How to Run Any Example

Each section folder contains one or more project folders. To run an example:

```bash
cd Section-X-<Topic>/<ExampleName>
dotnet run
```

**Tip:** Start with Section-1 and progress sequentially. Each builds conceptual knowledge for the next.

---

## 🔧 Key Technologies

- **[Microsoft.Agents.AI](https://github.com/microsoft/agents)** – Core agents framework
- **[Ollama](https://ollama.ai)** – Local LLM runtime (run models on your machine)
- **[OllamaSharp](https://github.com/awaesomeo/OllamaSharp)** – .NET client for Ollama
- **[.NET 10+](https://dotnet.microsoft.com/en-us/download)** – Language and runtime
- **[Qdrant](https://qdrant.tech/)** (Section 6) – Vector store for RAG

---

## 💡 Learning Outcomes

After working through this repository, you'll be able to:

✅ Design and build AI agents from scratch  
✅ Equip agents with tools and external capabilities  
✅ Manage agent memory and conversational context  
✅ Orchestrate complex multi-step workflows  
✅ Integrate knowledge retrieval (RAG) into agents  
✅ Build systems where agents collaborate  
✅ Extend agents with MCP servers

---

## 📖 Additional Resources

- [Microsoft Agents Framework Docs](https://github.com/microsoft/agent-framework)
- [Ollama Documentation](https://github.com/ollama/ollama)
- [Model Context Protocol](https://spec.modelcontextprotocol.io/)
