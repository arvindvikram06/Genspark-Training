import React, { useState } from 'react';
import { Globe, ArrowRight, Server, Box, Layers, MousePointerClick, ShieldCheck, Cpu } from 'lucide-react';

export default function NetworkingSimulator() {
  const [activeStep, setActiveStep] = useState(0);
  const [isAnimating, setIsAnimating] = useState(false);

  const simulateTraffic = () => {
    if (isAnimating) return;
    setIsAnimating(true);
    setActiveStep(1); // User hits Ingress

    setTimeout(() => {
      setActiveStep(2); // Ingress routes to Service
    }, 1500);

    setTimeout(() => {
      setActiveStep(3); // Service load balances to a Pod
    }, 3000);

    setTimeout(() => {
      setActiveStep(0); // Reset
      setIsAnimating(false);
    }, 5000);
  };

  return (
    <div className="min-h-screen bg-slate-50 text-slate-800 p-4 md:p-8 font-sans">
      <div className="max-w-6xl mx-auto space-y-6">
        
        {/* Header */}
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center bg-white p-6 rounded-xl shadow-sm border border-slate-200 gap-4">
          <div>
            <h1 className="text-2xl font-bold text-slate-900 flex items-center gap-2">
              <Globe className="w-6 h-6 text-blue-600" />
              Kubernetes Networking & Traffic Flow
            </h1>
            <p className="text-slate-500 text-sm mt-1">Watch how an external user request travels through the cluster to reach a Pod.</p>
          </div>
          <div>
            <button 
              onClick={simulateTraffic}
              disabled={isAnimating}
              className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg flex items-center gap-2 transition-colors disabled:opacity-50 shadow-sm shadow-blue-200"
            >
              <MousePointerClick className="w-4 h-4" />
              Simulate User Request
            </button>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          
          {/* Main Visualization Area */}
          <div className="lg:col-span-2 space-y-12 relative bg-white p-8 rounded-2xl border-2 border-slate-200 shadow-sm">
            
            {/* 1. Internet / User */}
            <div className="flex justify-center relative">
              <div className={`p-4 rounded-xl border-2 flex items-center gap-3 transition-all duration-500 z-10 bg-slate-100 border-slate-300`}>
                <Globe className={`w-8 h-8 ${activeStep === 1 ? 'text-blue-500 animate-pulse' : 'text-slate-500'}`} />
                <div className="font-bold">External User</div>
              </div>
              
              {activeStep === 1 && (
                <div className="absolute top-16 z-0 flex flex-col items-center animate-bounce">
                   <div className="bg-blue-500 text-white text-xs px-2 py-1 rounded font-bold">DNS Lookup</div>
                   <ArrowRight className="w-6 h-6 text-blue-500 transform rotate-90 my-1" />
                </div>
              )}
            </div>

            {/* 2. Ingress Controller */}
            <div className="flex justify-center relative mt-8">
              <div className={`w-full max-w-md p-6 rounded-2xl border-2 border-dashed flex flex-col items-center gap-3 transition-all duration-500 z-10 relative
                ${activeStep === 1 || activeStep === 2 ? 'bg-purple-50 border-purple-400 shadow-md ring-4 ring-purple-100' : 'bg-slate-50 border-slate-300'}`}
              >
                <ShieldCheck className={`w-10 h-10 ${activeStep === 1 || activeStep === 2 ? 'text-purple-600' : 'text-slate-400'}`} />
                <div className="text-center">
                  <div className="font-bold text-lg text-slate-800">Ingress Controller</div>
                  <div className="text-xs text-slate-500 mt-1">Nginx / Traefik / ALB</div>
                  <div className="text-xs font-mono bg-white px-2 py-1 rounded mt-2 border border-slate-200">Rules: path /api {"->"} backend-service</div>
                </div>
              </div>

              {activeStep === 2 && (
                <div className="absolute top-36 z-0 flex flex-col items-center animate-bounce">
                   <div className="bg-purple-500 text-white text-xs px-2 py-1 rounded font-bold">Routing Rule Matched</div>
                   <ArrowRight className="w-6 h-6 text-purple-500 transform rotate-90 my-1" />
                </div>
              )}
            </div>

            {/* 3. Service */}
            <div className="flex justify-center relative mt-8">
              <div className={`w-full max-w-sm p-4 rounded-full border-2 flex items-center justify-center gap-3 transition-all duration-500 z-10 relative
                ${activeStep === 2 || activeStep === 3 ? 'bg-emerald-50 border-emerald-400 shadow-md ring-4 ring-emerald-100' : 'bg-slate-50 border-slate-300'}`}
              >
                <Layers className={`w-6 h-6 ${activeStep === 2 || activeStep === 3 ? 'text-emerald-600' : 'text-slate-400'}`} />
                <div className="font-bold text-emerald-900">ClusterIP Service</div>
              </div>

              {activeStep === 3 && (
                <div className="absolute top-16 z-0 flex flex-col items-center animate-bounce">
                   <div className="bg-emerald-500 text-white text-xs px-2 py-1 rounded font-bold">Load Balancing</div>
                   <ArrowRight className="w-6 h-6 text-emerald-500 transform rotate-90 my-1" />
                </div>
              )}
            </div>

            {/* 4. Pods */}
            <div className="grid grid-cols-3 gap-4 mt-8">
              {[1, 2, 3].map((podNum) => (
                <div key={podNum} className={`p-4 rounded-xl border-2 flex flex-col items-center justify-center gap-2 transition-all duration-500
                  ${activeStep === 3 && podNum === 2 ? 'bg-blue-50 border-blue-500 shadow-lg scale-105 ring-4 ring-blue-100' : 'bg-slate-50 border-slate-200'}`}
                >
                  <Box className={`w-8 h-8 ${activeStep === 3 && podNum === 2 ? 'text-blue-600' : 'text-slate-400'}`} />
                  <div className="text-sm font-bold">Pod {podNum}</div>
                  <div className="text-xs text-slate-500">Port 8080</div>
                  {activeStep === 3 && podNum === 2 && (
                    <div className="mt-2 text-xs font-bold text-blue-600 animate-pulse">Processing Request...</div>
                  )}
                </div>
              ))}
            </div>

          </div>

          {/* Explanation Panel */}
          <div className="bg-slate-900 rounded-2xl shadow-lg border border-slate-800 flex flex-col">
            <div className="bg-slate-950 px-4 py-4 border-b border-slate-800 flex items-center gap-2 rounded-t-2xl">
              <Server className="w-5 h-5 text-slate-400" />
              <h2 className="text-sm font-bold text-slate-200 tracking-wider">NETWORKING CONCEPTS</h2>
            </div>
            <div className="p-6 space-y-6 text-slate-300 text-sm">
              <div>
                <h3 className="text-white font-bold mb-2 flex items-center gap-2">
                  <span className="bg-blue-500 text-white w-6 h-6 rounded-full flex items-center justify-center text-xs">1</span>
                  The Ingress Controller
                </h3>
                <p>The Ingress acts as the "front door" to your cluster. It sits on the edge, holding a public IP address. It reads the HTTP host and path (e.g., `api.example.com/users`) and uses rules to decide which internal Service to forward traffic to.</p>
              </div>
              
              <div>
                <h3 className="text-white font-bold mb-2 flex items-center gap-2">
                  <span className="bg-purple-500 text-white w-6 h-6 rounded-full flex items-center justify-center text-xs">2</span>
                  The Service (ClusterIP)
                </h3>
                <p>Because Pods are constantly dying and being recreated with new IP addresses (as seen in the Lifecycle Simulator), you cannot rely on a Pod's IP. The Service provides a stable, permanent internal IP address that never changes.</p>
              </div>

              <div>
                <h3 className="text-white font-bold mb-2 flex items-center gap-2">
                  <span className="bg-emerald-500 text-white w-6 h-6 rounded-full flex items-center justify-center text-xs">3</span>
                  Load Balancing
                </h3>
                <p>The Service doesn't just provide a static IP; it acts as an internal load balancer. Using `iptables` or IPVS, it takes incoming requests and distributes them evenly across all healthy Pods that match its label selector (e.g., `app: my-backend`).</p>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}
