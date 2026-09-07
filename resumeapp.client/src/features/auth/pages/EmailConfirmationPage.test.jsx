import { StrictMode } from 'react'
import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import {
    MemoryRouter,
    Route,
    Routes,
    useNavigationType,
} from 'react-router-dom'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import axiosClient from '../../../core/api/axiosClient'
import EmailConfirmationPage from './EmailConfirmationPage'

vi.mock('../../../core/api/axiosClient', () => ({
    default: {
        post: vi.fn(),
    },
}))

function DestinationProbe() {
    const navigationType = useNavigationType()

    return (
        <>
            <h1>Dashboard page</h1>
            <span data-testid="navigation-type">{navigationType}</span>
        </>
    )
}

function renderConfirmationPage(initialEntry) {
    return render(
        <StrictMode>
            <MemoryRouter initialEntries={[initialEntry]}>
                <Routes>
                    <Route
                        path="/confirm-email"
                        element={<EmailConfirmationPage />}
                    />
                    <Route
                        path="/dashboard"
                        element={<DestinationProbe />}
                    />
                    <Route path="/login" element={<p>Login page</p>} />
                </Routes>
            </MemoryRouter>
        </StrictMode>,
    )
}

// Verifies secure confirmation processing, storage, navigation, and recovery.
describe('EmailConfirmationPage', () => {
    beforeEach(() => {
        localStorage.clear()
        vi.clearAllMocks()
    })

    afterEach(() => {
        vi.restoreAllMocks()
    })

    it('shows processing and removes confirmation data before completion', async () => {
        axiosClient.post.mockReturnValue(new Promise(() => { }))
        const replaceState = vi.spyOn(window.history, 'replaceState')

        renderConfirmationPage(
            '/confirm-email?userId=user-id&token=sensitive-token',
        )

        expect(
            screen.getByRole('heading', { name: 'Confirming your email' }),
        ).toBeInTheDocument()
        expect(screen.getByText(
            'Please wait while we securely confirm your account.',
        )).toBeInTheDocument()

        await waitFor(() => {
            expect(replaceState).toHaveBeenCalled()
        })

        const scrubbedUrl = replaceState.mock.calls.at(-1)[2]

        expect(scrubbedUrl).toBe('/confirm-email')
        expect(scrubbedUrl).not.toContain('user-id')
        expect(scrubbedUrl).not.toContain('sensitive-token')
        expect(document.body).not.toHaveTextContent('sensitive-token')
        expect(axiosClient.post).toHaveBeenCalledTimes(1)
    })

    it('stores normal authentication data and replaces with the dashboard', async () => {
        axiosClient.post.mockResolvedValue({
            data: {
                token: 'test-jwt',
                email: 'user@example.com',
                fullName: 'Test User',
            },
        })

        renderConfirmationPage(
            '/confirm-email#userId=user-id&token=valid-token',
        )

        expect(
            await screen.findByRole('heading', { name: 'Dashboard page' }),
        ).toBeInTheDocument()
        expect(axiosClient.post).toHaveBeenCalledWith(
            '/auth/confirm-email',
            {
                userId: 'user-id',
                token: 'valid-token',
            },
        )
        expect(localStorage.getItem('resume_app_token')).toBe('test-jwt')
        expect(localStorage.getItem('user_email')).toBe('user@example.com')
        expect(localStorage.getItem('user_name')).toBe('Test User')
        expect(screen.getByTestId('navigation-type')).toHaveTextContent(
            'REPLACE',
        )
    })

    it('shows generic failure guidance without authenticating', async () => {
        axiosClient.post.mockRejectedValue({
            response: {
                data: 'test-only-technical-detail',
            },
        })

        renderConfirmationPage(
            '/confirm-email?userId=user-id&token=invalid-token',
        )

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'We could not confirm this email link. It may be invalid, expired, or already used.',
        )
        expect(document.body).not.toHaveTextContent(
            'test-only-technical-detail',
        )
        expect(localStorage.getItem('resume_app_token')).toBeNull()
        expect(
            screen.getByRole('link', { name: 'Return to login' }),
        ).toHaveAttribute('href', '/login')
        expect(
            screen.getByRole('button', {
                name: 'Request another confirmation email',
            }),
        ).toBeInTheDocument()
    })

    it('rejects missing confirmation data without calling the API', async () => {
        renderConfirmationPage('/confirm-email')

        expect(await screen.findByRole('alert')).toBeInTheDocument()
        expect(axiosClient.post).not.toHaveBeenCalled()
        expect(localStorage).toHaveLength(0)
    })

    it('requests another confirmation email with generic feedback', async () => {
        axiosClient.post
            .mockRejectedValueOnce(new Error('confirmation failed'))
            .mockResolvedValueOnce({
                data: 'If an unverified account exists, a link was generated.',
            })

        renderConfirmationPage(
            '/confirm-email?userId=user-id&token=invalid-token',
        )

        await screen.findByRole('alert')

        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: '  user@example.com  ' },
        })
        fireEvent.click(
            screen.getByRole('button', {
                name: 'Request another confirmation email',
            }),
        )

        expect(axiosClient.post).toHaveBeenLastCalledWith(
            '/auth/resend-email-confirmation',
            { email: 'user@example.com' },
        )
        expect(await screen.findByText(
            'If an unverified account exists, a new confirmation email has been requested.',
        )).toBeInTheDocument()
    })

    it('does not store authentication when the API omits a token', async () => {
        axiosClient.post.mockResolvedValue({
            data: {
                email: 'user@example.com',
                fullName: 'Test User',
            },
        })

        renderConfirmationPage(
            '/confirm-email?userId=user-id&token=valid-token',
        )

        expect(await screen.findByRole('alert')).toBeInTheDocument()
        expect(localStorage).toHaveLength(0)
        expect(
            screen.queryByRole('heading', { name: 'Dashboard page' }),
        ).not.toBeInTheDocument()
    })
})
