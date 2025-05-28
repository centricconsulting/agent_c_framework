// src/Routes.jsx
import React, {Suspense, lazy} from 'react';
import {Routes, Route} from 'react-router-dom';
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';

// Lazy load pages
const HomePage = lazy(() => import('@/pages/HomePage'));
const ChatPage = lazy(() => import('@/pages/ChatPage'));
const SettingsPage = lazy(() => import('@/pages/SettingsPage'));
const RagPage = lazy(() => import('@/pages/RAGPage'));
const InteractionsPage = lazy(() => import('@/components/replay_interface/InteractionsPage'));
const ReplayPage = lazy(() => import('@/components/replay_interface/ReplayPage'));
const LoginPage = lazy(() => import('@/pages/LoginPage'));

const AppRoutes = () => {
    return (
        <Suspense fallback={<div>Loading...</div>}>
            <Routes>
                {/* Public route */}
                <Route path="/login" element={<LoginPage />} />
                
                {/* Protected routes */}
                <Route path="/" element={
                    <ProtectedRoute>
                        <Layout>
                            <HomePage />
                        </Layout>
                    </ProtectedRoute>
                } />
                <Route path="/home" element={
                    <ProtectedRoute>
                        <Layout>
                            <ChatPage />
                        </Layout>
                    </ProtectedRoute>
                } />
                <Route path="/chat" element={
                    <ProtectedRoute>
                        <Layout>
                            <ChatPage />
                        </Layout>
                    </ProtectedRoute>
                } />
                <Route path="/settings" element={
                    <ProtectedRoute>
                        <Layout>
                            <SettingsPage />
                        </Layout>
                    </ProtectedRoute>
                } />
                <Route path="/rag" element={
                    <ProtectedRoute>
                        <Layout>
                            <RagPage />
                        </Layout>
                    </ProtectedRoute>
                } />
                <Route path="/interactions" element={
                    <ProtectedRoute>
                        <Layout>
                            <InteractionsPage />
                        </Layout>
                    </ProtectedRoute>
                } />
                <Route path="/replay/:sessionId" element={
                    <ProtectedRoute>
                        <Layout>
                            <ReplayPage />
                        </Layout>
                    </ProtectedRoute>
                } />
            </Routes>
        </Suspense>
    );
};

export default AppRoutes;