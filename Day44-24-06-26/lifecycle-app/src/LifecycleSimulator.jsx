import React, { useState, useEffect, useRef } from 'react';
import { Server, Box, Activity, Database, Cpu, Network, ArrowRight, Play, AlertOctagon, CheckCircle2, Loader2, Info } from 'lucide-react';

export default function LifecycleSimulator() {
  const [isSimulating, setIsSimulating] = useState(false);
  const [activeComponent, setActiveComponent] = useState(null);
  const [logs, setLogs] = useState([{ id: 0, text: "Cluster initialized. Desired state: 3 Pods.", type: "info" }]);
  const [trafficTarget, setTrafficTarget] = useState(null);

  const [nodes, setNodes] = useState([
    { id: 'node-1', name: 'Worker Node A', active: true },
    { id: 'node-2', name: 'Worker Node B', active: true }
  ]);

  const [pods, setPods] = useState([
    { id: 'pod-a1', nodeId: 'node-1', status: 'running' },
    { id: 'pod-b2', nodeId: 'node-1', status: 'running' },
    { id: 'pod-c3', nodeId: 'node-2', status: 'running' }
  ]);

  const logsEndRef = useRef(null);

  useEffect(() => {
    logsEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [logs]);

  const addLog = (text, type = "info") => {
    setLogs(prev => [...prev, { id: Date.now(), text, type }]);
  };

  const generateId = () => Math.random().toString(36).substring(2, 6);

  const simulateTraffic = () => {
    if (isSimulating) return;

    const runningPods = pods.filter(p => p.status === 'running');
    if (runningPods.length === 0) {
      addLog("Traffic failed. No healthy pods available!", "error");
      return;
    }

    const randomPod = runningPods[Math.floor(Math.random() * runningPods.length)];
    setTrafficTarget(randomPod.id);
    addLog(`Ingress routed external traffic to ${randomPod.id}`, "success");

    setTimeout(() => {
      setTrafficTarget(null);
    }, 1000);
  };

  const simulatePodCrash = () => {
    if (isSimulating) return;
    setIsSimulating(true);
    setTrafficTarget(null);

    const runningPods = pods.filter(p => p.status === 'running');
    if (runningPods.length === 0) {
      setIsSimulating(false);
      return;
    }

    // 1. Pick a random pod to crash
    const podToCrash = runningPods[Math.floor(Math.random() * runningPods.length)];
    const targetNodeForNewPod = podToCrash.nodeId === 'node-1' ? 'node-2' : 'node-1'; // Put new pod on the other node for visual clarity
    const newPodId = `pod-${generateId()}`;

    // Step 1: Crash
    setPods(prev => prev.map(p => p.id === podToCrash.id ? { ...p, status: 'dead' } : p));
    addLog(`🚨 Pod ${podToCrash.id} crashed due to OutOfMemory error.`, "error");
    setActiveComponent('kubelet-' + podToCrash.nodeId);

    // Step 2: API Server receives update
    setTimeout(() => {
      setActiveComponent('api-server');
      addLog(`📡 Kubelet reports pod failure to API Server.`, "info");
    }, 2000);

    // Step 3: Controller Manager detects mismatch
    setTimeout(() => {
      setActiveComponent('controller-manager');
      addLog(`⚙️ Controller Manager detects mismatch (Actual: 2, Desired: 3). Requests new Pod.`, "warning");
    }, 4000);

    // Step 4: Scheduler assigns node
    setTimeout(() => {
      setActiveComponent('scheduler');
      addLog(`🧠 Scheduler finds available CPU/Mem on ${targetNodeForNewPod === 'node-1' ? 'Worker Node A' : 'Worker Node B'}. Assigns new pod.`, "info");
    }, 6000);

    // Step 5: New pod is pending on new node
    setTimeout(() => {
      setActiveComponent('kubelet-' + targetNodeForNewPod);
      setPods(prev => [...prev, { id: newPodId, nodeId: targetNodeForNewPod, status: 'pending' }]);
      addLog(`🏗️ Kubelet receives instructions. Pulling container image for ${newPodId}...`, "warning");
    }, 8000);

    // Step 6: New pod running, old pod garbage collected
    setTimeout(() => {
      setActiveComponent(null);
      setPods(prev => {
        // Remove dead pod, set new pod to running
        const filtered = prev.filter(p => p.id !== podToCrash.id);
        return filtered.map(p => p.id === newPodId ? { ...p, status: 'running' } : p);
      });
      addLog(`✅ ${newPodId} is Running. Dead pod garbage collected. Desired state restored.`, "success");
      setIsSimulating(false);
    }, 11000);
  };

  const getComponentRing = (id) => {
    return activeComponent === id ? "ring-4 ring-blue-500 animate-pulse bg-blue-50 border-blue-400" : "border-slate-200 bg-white";
  };

  return (
    <div className="min-h-screen bg-slate-50 text-slate-800 p-4 md:p-8 font-sans">
      <div className="max-w-6xl mx-auto space-y-6">

        {/* Header & Controls */}
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center bg-white p-6 rounded-xl shadow-sm border border-slate-200 gap-4">
          <div>
            <h1 className="text-2xl font-bold text-slate-900 flex items-center gap-2">
              <Network className="w-6 h-6 text-blue-600" />
              Kubernetes Rescheduling Simulator
            </h1>
            <p className="text-slate-500 text-sm mt-1">Watch how the Control Plane replaces dead pods (they do not move!).</p>
          </div>
          <div className="flex gap-3">
            <button
              onClick={simulateTraffic}
              disabled={isSimulating}
              className="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 font-medium rounded-lg flex items-center gap-2 transition-colors disabled:opacity-50"
            >
              <ArrowRight className="w-4 h-4" />
              Send Traffic
            </button>
            <button
              onClick={simulatePodCrash}
              disabled={isSimulating}
              className="px-4 py-2 bg-red-600 hover:bg-red-700 text-white font-medium rounded-lg flex items-center gap-2 transition-colors disabled:opacity-50 shadow-sm shadow-red-200"
            >
              <AlertOctagon className="w-4 h-4" />
              Kill Random Pod
            </button>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

          {/* Main Visualization Area */}
          <div className="lg:col-span-2 space-y-8 relative">

            {/* CONTROL PLANE */}
            <div className="bg-slate-100 p-6 rounded-2xl border-2 border-dashed border-slate-300 relative">
              <div className="absolute -top-3 left-6 bg-slate-100 px-2 text-xs font-bold tracking-widest text-slate-400 uppercase">Control Plane (The Brain)</div>

              <div className="grid grid-cols-3 gap-4 text-center">
                <div className={`p-4 rounded-xl border-2 flex flex-col items-center justify-center gap-2 transition-all duration-300 ${getComponentRing('controller-manager')}`}>
                  <Activity className={`w-8 h-8 ${activeComponent === 'controller-manager' ? 'text-blue-600' : 'text-slate-400'}`} />
                  <span className="text-sm font-semibold">Controller Manager</span>
                  <span className="text-xs text-slate-500">Monitors Desired vs Actual State</span>
                </div>

                <div className={`p-4 rounded-xl border-2 flex flex-col items-center justify-center gap-2 transition-all duration-300 z-10 shadow-md ${getComponentRing('api-server')}`}>
                  <Server className={`w-8 h-8 ${activeComponent === 'api-server' ? 'text-blue-600' : 'text-slate-600'}`} />
                  <span className="text-sm font-bold text-slate-800">API Server</span>
                  <span className="text-xs text-slate-500">The central communication hub</span>
                </div>

                <div className={`p-4 rounded-xl border-2 flex flex-col items-center justify-center gap-2 transition-all duration-300 ${getComponentRing('scheduler')}`}>
                  <Cpu className={`w-8 h-8 ${activeComponent === 'scheduler' ? 'text-blue-600' : 'text-slate-400'}`} />
                  <span className="text-sm font-semibold">Scheduler</span>
                  <span className="text-xs text-slate-500">Assigns Pods to Nodes</span>
                </div>
              </div>
            </div>

            {/* Service / Load Balancer */}
            <div className="flex justify-center">
              <div className="bg-emerald-50 border-2 border-emerald-200 px-8 py-3 rounded-full flex items-center gap-3 shadow-sm z-10">
                <Network className="w-5 h-5 text-emerald-600" />
                <span className="font-semibold text-emerald-800">Cluster Service (Load Balancer)</span>
              </div>
            </div>

            {/* WORKER NODES */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {nodes.map(node => (
                <div key={node.id} className="bg-white p-5 rounded-2xl border-2 border-slate-200 shadow-sm relative">
                  <div className="flex justify-between items-center mb-4 border-b border-slate-100 pb-3">
                    <h3 className="font-bold text-slate-700 flex items-center gap-2">
                      <Server className="w-5 h-5 text-slate-400" />
                      {node.name}
                    </h3>
                    <div className={`text-xs font-semibold px-2 py-1 rounded flex items-center gap-1 transition-all duration-300 ${getComponentRing('kubelet-' + node.id)}`}>
                      <Activity className="w-3 h-3" />
                      Kubelet Agent
                    </div>
                  </div>

                  <div className="grid grid-cols-2 gap-3 min-h-[120px]">
                    {pods.filter(p => p.nodeId === node.id).map(pod => (
                      <div
                        key={pod.id}
                        className={`
                          relative p-3 rounded-lg border-2 flex flex-col items-center justify-center gap-2 transition-all duration-500
                          ${pod.status === 'running' ? (trafficTarget === pod.id ? 'bg-emerald-100 border-emerald-500 shadow-lg scale-105' : 'bg-emerald-50 border-emerald-200') : ''}
                          ${pod.status === 'dead' ? 'bg-red-50 border-red-300 opacity-70 grayscale' : ''}
                          ${pod.status === 'pending' ? 'bg-yellow-50 border-yellow-300 animate-pulse' : ''}
                        `}
                      >
                        {pod.status === 'running' && <Box className="w-6 h-6 text-emerald-600" />}
                        {pod.status === 'dead' && <AlertOctagon className="w-6 h-6 text-red-500" />}
                        {pod.status === 'pending' && <Loader2 className="w-6 h-6 text-yellow-600 animate-spin" />}

                        <div className="text-center">
                          <div className="text-sm font-bold font-mono">{pod.id}</div>
                          <div className={`text-xs font-semibold capitalize 
                            ${pod.status === 'running' ? 'text-emerald-700' : ''}
                            ${pod.status === 'dead' ? 'text-red-600' : ''}
                            ${pod.status === 'pending' ? 'text-yellow-700' : ''}
                          `}>
                            {pod.status}
                          </div>
                        </div>

                        {/* Traffic indicator line */}
                        {trafficTarget === pod.id && (
                          <div className="absolute -top-12 left-1/2 w-0.5 h-12 bg-emerald-400 animate-pulse"></div>
                        )}
                      </div>
                    ))}

                    {pods.filter(p => p.nodeId === node.id).length === 0 && (
                      <div className="col-span-2 flex items-center justify-center text-slate-400 text-sm italic h-full border-2 border-dashed border-slate-100 rounded-lg">
                        No pods running
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>

            {/* Explainer Callout */}
            <div className="bg-blue-50 border border-blue-200 rounded-xl p-4 flex gap-3 text-blue-800 text-sm">
              <Info className="w-5 h-5 flex-shrink-0 mt-0.5" />
              <div>
                <strong>Notice how the pods behave:</strong> When a pod crashes, it doesn't "move" to the other node. It dies permanently. The Controller Manager detects that there are only 2 pods instead of 3, and creates a completely new, fresh Pod clone from the deployment template.
              </div>
            </div>

          </div>

          {/* Logs Panel */}
          <div className="bg-slate-900 rounded-2xl shadow-lg border border-slate-800 flex flex-col h-[600px] lg:h-auto overflow-hidden">
            <div className="bg-slate-950 px-4 py-3 border-b border-slate-800 flex items-center gap-2">
              <Database className="w-4 h-4 text-slate-400" />
              <h2 className="text-sm font-bold text-slate-200 font-mono tracking-wider">CLUSTER EVENTS LOG</h2>
            </div>
            <div className="flex-1 p-4 overflow-y-auto font-mono text-sm space-y-3">
              {logs.map((log) => (
                <div key={log.id} className={`flex items-start gap-2 animate-in slide-in-from-left-2 duration-300
                  ${log.type === 'error' ? 'text-red-400' : ''}
                  ${log.type === 'warning' ? 'text-yellow-400' : ''}
                  ${log.type === 'success' ? 'text-emerald-400' : ''}
                  ${log.type === 'info' ? 'text-blue-300' : ''}
                `}>
                  <span className="text-slate-600 text-xs mt-0.5">[{new Date(log.id).toLocaleTimeString([], { hour12: false, second: '2-digit' })}]</span>
                  <span className="leading-tight">{log.text}</span>
                </div>
              ))}
              <div ref={logsEndRef} />
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}