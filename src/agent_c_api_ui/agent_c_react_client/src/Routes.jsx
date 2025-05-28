// src/Routes.jsx
import React, {Suspense, lazy} from 'react';
import {Routes, Route} from 'react-router-dom';
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import { protectedRoutes } from './config/authConfig';

// Lazy load pages
const HomePage = lazy(() => import('@/pages/HomePage'));
const ChatPage = lazy(() => import('@/pages/ChatPage'));
const SettingsPage = lazy(() => import('@/pages/SettingsPage'));
const RagPage = lazy(() => import('@/pages/RAGPage'));
const InteractionsPage = lazy(() => import('@/components/replay_interface/InteractionsPage'));
const ReplayPage = lazy(() => import('@/components/replay_interface/ReplayPage'));

const AppRoutes = () => {
    return (
        <Layout>
            <Suspense fallback={<div>Loading...</div>}>
                <Routes>
                    {/* Public routes */}
                    <Route path="/" element={<HomePage/>}/>
                    <Route path="/home" element={<HomePage/>}/>
                    
                    {/* Protected routes */}
                    <Route path="/chat" element={
                        <ProtectedRoute>
                            <ChatPage/>
                        </ProtectedRoute>
                    }/>
                    <Route path="/settings" element={
                        <ProtectedRoute>
                            <SettingsPage/>
                        </ProtectedRoute>
                    }/>
                    <Route path="/rag" element={
                        <ProtectedRoute>
                            <RagPage/>
                        </ProtectedRoute>
                    }/>
                    <Route path="/interactions" element={
                        <ProtectedRoute>
                            <InteractionsPage/>
                        </ProtectedRoute>
                    }/>
                    <Route path="/replay/:sessionId" element={
                        <ProtectedRoute>
                            <ReplayPage />
                        </ProtectedRoute>
                    }/>
                </Routes>
            </Suspense>
        </Layout>
    );
};

export default AppRoutes;