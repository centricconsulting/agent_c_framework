// src/Routes.jsx
import React, {Suspense, lazy} from 'react';
import {Routes, Route} from 'react-router-dom';
import Layout from './components/Layout';

// Lazy load pages
const HomePage = lazy(() => import('@/pages/HomePage'));
const ChatPage = lazy(() => import('@/pages/ChatPage'));
const SettingsPage = lazy(() => import('@/pages/SettingsPage'));
const LoginPage = lazy(() => import('@/pages/LoginPage'));
const RagPage = lazy(() => import('@/pages/RAGPage'));
const InteractionsPage = lazy(() => import('@/components/replay_interface/InteractionsPage'));
const ReplayPage = lazy(() => import('@/components/replay_interface/ReplayPage'));

const AppRoutes = () => {
    return (
        <Layout>
            <Suspense fallback={<div>Loading...</div>}>
                <Routes>
                    <Route path="/home" element={<ChatPage/>}/>
                    <Route path="/chat" element={<ChatPage/>}/>
                    <Route path="/" element={<HomePage/>}/>
                    <Route path="/settings" element={<SettingsPage/>}/>
                    <Route path="/login" element={<LoginPage/>}/>
                    <Route path="/rag" element={<RagPage/>}/>
                    <Route path="/interactions" element={<InteractionsPage/>}/>
                    <Route path="/replay/:sessionId" element={<ReplayPage />} />
                </Routes>
            </Suspense>
        </Layout>
    );
};

export default AppRoutes;