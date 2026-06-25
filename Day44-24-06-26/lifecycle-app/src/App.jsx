import React, { useState } from 'react';
import LifecycleSimulator from './LifecycleSimulator';
import NetworkingSimulator from './NetworkingSimulator';
import { Layers, Activity } from 'lucide-react';

export default function App() {
  const [activeTab, setActiveTab] = useState('lifecycle');

  return (
    <div className="min-h-screen bg-slate-100 flex flex-col font-sans text-slate-800">
      
      {/* Top Navigation */}
      <nav className="bg-slate-900 text-white shadow-lg border-b border-slate-800 sticky top-0 z-50">
        <div className="max-w-6xl mx-auto px-4 md:px-8 flex flex-col sm:flex-row justify-between items-center h-auto sm:h-16 py-4 sm:py-0 gap-4">
          <div className="font-bold text-xl flex items-center gap-2 tracking-wide">
            <span className="text-blue-400 font-mono font-black">K8S</span> 
            Interactive Lab
          </div>
          
          <div className="flex bg-slate-800 p-1 rounded-lg">
            <button 
              onClick={() => setActiveTab('lifecycle')}
              className={`px-4 py-2 rounded-md font-medium text-sm transition-all flex items-center gap-2
                ${activeTab === 'lifecycle' ? 'bg-blue-600 text-white shadow-md' : 'text-slate-300 hover:text-white hover:bg-slate-700'}`}
            >
              <Activity className="w-4 h-4" />
              Pod Lifecycle
            </button>
            <button 
              onClick={() => setActiveTab('networking')}
              className={`px-4 py-2 rounded-md font-medium text-sm transition-all flex items-center gap-2
                ${activeTab === 'networking' ? 'bg-blue-600 text-white shadow-md' : 'text-slate-300 hover:text-white hover:bg-slate-700'}`}
            >
              <Layers className="w-4 h-4" />
              Networking
            </button>
          </div>
        </div>
      </nav>

      {/* Main Content Area */}
      <main className="flex-1">
        {activeTab === 'lifecycle' && <LifecycleSimulator />}
        {activeTab === 'networking' && <NetworkingSimulator />}
      </main>
      
    </div>
  );
}