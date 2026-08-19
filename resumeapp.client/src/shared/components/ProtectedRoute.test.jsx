import { render, screen } from '@testing-library/react'
import {MemoryRouter,Route,Routes,useLocation,useNavigationType,} from 'react-router-dom'
import { beforeEach, describe, expect, it } from 'vitest'
import ProtectedRoute from './ProtectedRoute'

function TestPage({ name }) {
    const location = useLocation()
    const navigationType = useNavigationType()

    return (
        <>
            <h1>{name}</h1>
            <p>Path: {location.pathname}</p>
            <p>Navigation: {navigationType}</p>
        </>
    )
}

function renderProtectedRoute(initialEntry = '/dashboard') {
    return render(
        <MemoryRouter initialEntries={[initialEntry]}>
            <Routes>
                <Route
                    path="/login"
                    element={<TestPage name="Login page" />}
                />

                <Route element={<ProtectedRoute />}>
                    <Route
                        path="/dashboard"
                        element={<TestPage name="Dashboard page" />}
                    />
                    <Route
                        path="/my-resume"
                        element={<TestPage name="Resume editor page" />}
                    />
                    <Route
                        path="/my-resume/preview"
                        element={<TestPage name="Resume preview page" />}
                    />
                </Route>
            </Routes>
        </MemoryRouter>,
    )
}

describe('ProtectedRoute', () => {
    beforeEach(() => {
        localStorage.clear()
    })

    // Verifies authentication redirects and access to protected child routes.
    it('redirects an unauthenticated user to login', async () => {
        renderProtectedRoute()

        expect(
            await screen.findByRole('heading', { name: 'Login page' }),
        ).toBeInTheDocument()
        expect(screen.getByText('Path: /login')).toBeInTheDocument()
    })

    it('redirects when the saved token is empty', async () => {
        localStorage.setItem('resume_app_token', '')

        renderProtectedRoute()

        expect(
            await screen.findByRole('heading', { name: 'Login page' }),
        ).toBeInTheDocument()
    })

    it('uses replace navigation when redirecting to login', async () => {
        renderProtectedRoute()

        expect(
            await screen.findByText('Navigation: REPLACE'),
        ).toBeInTheDocument()
    })

    it('renders the dashboard when a token exists', () => {
        localStorage.setItem('resume_app_token', 'test-token')

        renderProtectedRoute()

        expect(
            screen.getByRole('heading', { name: 'Dashboard page' }),
        ).toBeInTheDocument()
        expect(screen.getByText('Path: /dashboard')).toBeInTheDocument()
        expect(
            screen.queryByRole('heading', { name: 'Login page' }),
        ).not.toBeInTheDocument()
    })

    it('renders another protected child route through the outlet', () => {
        localStorage.setItem('resume_app_token', 'test-token')

        renderProtectedRoute('/my-resume/preview')

        expect(
            screen.getByRole('heading', { name: 'Resume preview page' }),
        ).toBeInTheDocument()
        expect(
            screen.getByText('Path: /my-resume/preview'),
        ).toBeInTheDocument()
    })

    it('does not remove or modify the authentication token', () => {
        localStorage.setItem('resume_app_token', 'existing-token')

        renderProtectedRoute('/my-resume')

        expect(
            screen.getByRole('heading', { name: 'Resume editor page' }),
        ).toBeInTheDocument()
        expect(localStorage.getItem('resume_app_token')).toBe(
            'existing-token',
        )
    })
})