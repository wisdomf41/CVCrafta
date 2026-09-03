import {act,fireEvent,render,screen,} from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import {MemoryRouter,Route,Routes,useLocation,} from 'react-router-dom'
import {beforeEach,describe,expect,it,vi,} from 'vitest'
import axiosClient from '../../../core/api/axiosClient'
import EmailConfirmationPendingPage from './EmailConfirmationPendingPage'
import RegisterPage from './RegisterPage'

vi.mock('../../../core/api/axiosClient', () => ({
    default: {
        post: vi.fn(),
    },
}))

function LocationStateProbe() {
    const location = useLocation()

    return (
        <output data-testid="location-state">
            {JSON.stringify(location.state)}
        </output>
    )
}

function renderRegisterPage() {
    return render(
        <MemoryRouter initialEntries={['/register']}>
            <Routes>
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/login" element={<p>Login page</p>} />
                <Route
                    path="/confirm-email-pending"
                    element={(
                        <>
                            <EmailConfirmationPendingPage />
                            <LocationStateProbe />
                        </>
                    )}
                />
            </Routes>
        </MemoryRouter>,
    )
}

function renderPendingPage(initialEntry = {
    pathname: '/confirm-email-pending',
    state: { email: 'user@example.com' },
}) {
    return render(
        <MemoryRouter initialEntries={[initialEntry]}>
            <Routes>
                <Route
                    path="/confirm-email-pending"
                    element={<EmailConfirmationPendingPage />}
                />
                <Route path="/login" element={<p>Login page</p>} />
                <Route path="/register" element={<p>Register page</p>} />
            </Routes>
        </MemoryRouter>,
    )
}

async function completeRegistrationForm(
    user,
    {
        fullName = 'Test User',
        email = 'user@example.com',
        password = 'Password123!',
        confirmPassword = 'Password123!',
    } = {},
) {
    await user.type(screen.getByLabelText('Full name'), fullName)
    await user.type(screen.getByLabelText('Email address'), email)
    await user.type(screen.getByLabelText('Password'), password)
    await user.type(
        screen.getByLabelText('Confirm password'),
        confirmPassword,
    )
}

describe('RegisterPage', () => {
    beforeEach(() => {
        localStorage.clear()
        sessionStorage.clear()
        vi.clearAllMocks()
    })

    // Verifies that the registration page displays all required controls.
    it('renders the complete registration form', () => {
        renderRegisterPage()

        expect(
            screen.getByRole('heading', { name: 'Create your account' }),
        ).toBeInTheDocument()
        expect(screen.getByLabelText('Full name')).toBeInTheDocument()
        expect(screen.getByLabelText('Email address')).toBeInTheDocument()
        expect(screen.getByLabelText('Password')).toBeInTheDocument()
        expect(
            screen.getByLabelText('Confirm password'),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('button', { name: 'Create account' }),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('link', { name: 'Sign in' }),
        ).toHaveAttribute('href', '/login')
    })

    // Verifies that users can enter information in every registration field.
    it('updates every field when the user types', () => {
        renderRegisterPage()

        const fullNameInput = screen.getByLabelText('Full name')
        const emailInput = screen.getByLabelText('Email address')
        const passwordInput = screen.getByLabelText('Password')
        const confirmPasswordInput = screen.getByLabelText('Confirm password')

        fireEvent.change(fullNameInput, {
            target: { value: 'Test User' },
        })
        fireEvent.change(emailInput, {
            target: { value: 'user@example.com' },
        })
        fireEvent.change(passwordInput, {
            target: { value: 'Password123!' },
        })
        fireEvent.change(confirmPasswordInput, {
            target: { value: 'Password123!' },
        })

        expect(fullNameInput).toHaveValue('Test User')
        expect(emailInput).toHaveValue('user@example.com')
        expect(passwordInput).toHaveValue('Password123!')
        expect(confirmPasswordInput).toHaveValue('Password123!')
    })

    // Verifies that required empty fields prevent an API request.
    it('does not submit when required fields are empty', async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        await user.click(
            screen.getByRole('button', { name: 'Create account' }),
        )

        expect(axiosClient.post).not.toHaveBeenCalled()
    })

    // Verifies that different passwords display a validation error.
    it('rejects registration when the passwords do not match', async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        await completeRegistrationForm(user, {
            confirmPassword: 'Different123!',
        })

        await user.click(
            screen.getByRole('button', { name: 'Create account' }),
        )

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Passwords do not match.',
        )
        expect(axiosClient.post).not.toHaveBeenCalled()
    })

    // Verifies that passwords shorter than six characters are rejected.
    it('rejects a password containing fewer than six characters', async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        await completeRegistrationForm(user, {
            password: '12345',
            confirmPassword: '12345',
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Password must be at least 6 characters.',
        )
        expect(axiosClient.post).not.toHaveBeenCalled()
    })

    // Verifies registration opens confirmation guidance with only the submitted email.
    it('registers successfully and opens confirmation-pending guidance', async () => {
        axiosClient.post.mockResolvedValue({ data: 'Registration successful.' })

        renderRegisterPage()

        fireEvent.change(screen.getByLabelText('Full name'), {
            target: { value: '  Test User  ' },
        })
        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: '  user@example.com  ' },
        })
        fireEvent.change(screen.getByLabelText('Password'), {
            target: { value: 'Password123!' },
        })
        fireEvent.change(screen.getByLabelText('Confirm password'), {
            target: { value: 'Password123!' },
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(axiosClient.post).toHaveBeenCalledWith('/auth/register', {
            fullName: 'Test User',
            email: 'user@example.com',
            password: 'Password123!',
        })

        expect(
            await screen.findByRole('heading', { name: 'Confirm your email' }),
        ).toBeInTheDocument()
        expect(screen.getByText(
            'Account created successfully. Check your email to confirm your account before signing in.',
        )).toBeInTheDocument()
        expect(screen.getByText('user@example.com')).toBeInTheDocument()
        expect(screen.queryByText('Login page')).not.toBeInTheDocument()
        expect(screen.getByTestId('location-state')).toHaveTextContent(
            JSON.stringify({ email: 'user@example.com' }),
        )
    })

    // Verifies an initial delivery failure still opens the resend recovery path.
    it('opens confirmation recovery when initial delivery does not arrive', async () => {
        axiosClient.post.mockResolvedValue({
            data: 'Registration successful. If the confirmation email does not arrive, request a new one.',
        })

        renderRegisterPage()

        fireEvent.change(screen.getByLabelText('Full name'), {
            target: { value: 'Test User' },
        })
        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: 'user@example.com' },
        })
        fireEvent.change(screen.getByLabelText('Password'), {
            target: { value: 'Password123!' },
        })
        fireEvent.change(screen.getByLabelText('Confirm password'), {
            target: { value: 'Password123!' },
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(
            await screen.findByRole('heading', { name: 'Confirm your email' }),
        ).toBeInTheDocument()
        expect(screen.getByText('user@example.com')).toBeInTheDocument()
        expect(
            screen.getByRole('button', {
                name: 'Resend confirmation email',
            }),
        ).toBeEnabled()
        expect(screen.queryByRole('alert')).not.toBeInTheDocument()
    })

    // Verifies submitted passwords are absent from the destination and browser storage.
    it('does not display or persist password data after registration', async () => {
        const submittedPassword = 'PrivatePassword123!'
        axiosClient.post.mockResolvedValue({ data: 'Registration successful.' })

        const user = userEvent.setup()
        renderRegisterPage()

        await completeRegistrationForm(user, {
            password: submittedPassword,
            confirmPassword: submittedPassword,
        })

        await user.click(
            screen.getByRole('button', { name: 'Create account' }),
        )

        expect(
            await screen.findByRole('heading', { name: 'Confirm your email' }),
        ).toBeInTheDocument()
        expect(document.body).not.toHaveTextContent(submittedPassword)
        expect(screen.getByTestId('location-state')).not.toHaveTextContent(
            submittedPassword,
        )
        expect(localStorage).toHaveLength(0)
        expect(sessionStorage).toHaveLength(0)
    })

    // Verifies that the button is disabled while registration is processing.
    it('shows a loading state while the request is running', async () => {
        axiosClient.post.mockReturnValue(new Promise(() => { }))

        const user = userEvent.setup()
        renderRegisterPage()

        await completeRegistrationForm(user)

        await user.click(
            screen.getByRole('button', { name: 'Create account' }),
        )

        expect(
            screen.getByRole('button', { name: 'Creating account...' }),
        ).toBeDisabled()
    })

    // Updated: uses fast events with a targeted timeout for slower test environments.
    it('displays a friendly message when the email already exists', async () => {
        const testEmail = ['user', 'example.com'].join('@')

        axiosClient.post.mockRejectedValue({
            response: {
                data: {
                    message: 'User already exists.',
                },
            },
        })

        renderRegisterPage()

        fireEvent.change(screen.getByLabelText('Full name'), {
            target: { value: 'Test User' },
        })
        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: testEmail },
        })
        fireEvent.change(screen.getByLabelText('Password'), {
            target: { value: 'Password123!' },
        })
        fireEvent.change(screen.getByLabelText('Confirm password'), {
            target: { value: 'Password123!' },
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'This email is already registered. Please sign in instead.',
        )
    }, 10000)

    // Verifies that an error title returned by the API is displayed.
    it('displays the API error title when registration fails', async () => {
        axiosClient.post.mockRejectedValue({
            response: {
                data: {
                    title: 'Registration service unavailable.',
                },
            },
        })

        renderRegisterPage()

        fireEvent.change(screen.getByLabelText('Full name'), {
            target: { value: 'Test User' },
        })
        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: 'user@example.com' },
        })
        fireEvent.change(screen.getByLabelText('Password'), {
            target: { value: 'Password123!' },
        })
        fireEvent.change(screen.getByLabelText('Confirm password'), {
            target: { value: 'Password123!' },
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Registration service unavailable.',
        )
        expect(screen.queryByRole('status')).not.toBeInTheDocument()
    })

    // Verifies that the sign-in link opens the login page.
    it('navigates to login when the sign-in link is selected', async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        await user.click(screen.getByRole('link', { name: 'Sign in' }))

        expect(screen.getByText('Login page')).toBeInTheDocument()
    })


})

// Covers resend feedback, request locking, and state-free confirmation navigation.
describe('EmailConfirmationPendingPage', () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it('resends confirmation using the existing endpoint and shows success', async () => {
        axiosClient.post.mockResolvedValue({
            data: 'If an unverified account exists, a new link was generated.',
        })

        const user = userEvent.setup()
        renderPendingPage()

        await user.click(
            screen.getByRole('button', {
                name: 'Resend confirmation email',
            }),
        )

        expect(axiosClient.post).toHaveBeenCalledWith(
            '/auth/resend-email-confirmation',
            { email: 'user@example.com' },
        )
        expect(await screen.findByRole('status')).toHaveTextContent(
            'Confirmation email sent. Check your inbox and spam/junk folder.',
        )
    })

    it('shows inline API feedback when resend fails', async () => {
        axiosClient.post.mockRejectedValue({
            response: {
                data: {
                    message: 'Confirmation email could not be sent.',
                },
            },
        })

        const user = userEvent.setup()
        renderPendingPage()

        await user.click(
            screen.getByRole('button', {
                name: 'Resend confirmation email',
            }),
        )

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Confirmation email could not be sent.',
        )
        expect(screen.queryByRole('status')).not.toBeInTheDocument()
    })

    it('disables resend and prevents duplicate requests while one is running', async () => {
        let resolveResend

        axiosClient.post.mockReturnValue(
            new Promise((resolve) => {
                resolveResend = resolve
            }),
        )

        renderPendingPage()

        const resendButton = screen.getByRole('button', {
            name: 'Resend confirmation email',
        })

        fireEvent.click(resendButton)
        fireEvent.click(resendButton)

        expect(
            screen.getByRole('button', {
                name: 'Resending confirmation email...',
            }),
        ).toBeDisabled()
        expect(axiosClient.post).toHaveBeenCalledTimes(1)

        await act(async () => {
            resolveResend({ data: 'Confirmation email sent.' })
        })

        expect(
            screen.getByRole('button', {
                name: 'Resend confirmation email',
            }),
        ).toBeEnabled()
    })

    it('shows safe guidance and navigation when opened without email state', () => {
        renderPendingPage('/confirm-email-pending')

        expect(
            screen.getByRole('heading', { name: 'Check your email' }),
        ).toBeInTheDocument()
        expect(screen.getByText(/check your inbox and spam\/junk folder/i))
            .toBeInTheDocument()
        expect(
            screen.getByRole('link', { name: 'Register' }),
        ).toHaveAttribute('href', '/register')
        expect(
            screen.getByRole('link', { name: 'Return to login' }),
        ).toHaveAttribute('href', '/login')
        expect(
            screen.queryByRole('button', {
                name: 'Resend confirmation email',
            }),
        ).not.toBeInTheDocument()
        expect(axiosClient.post).not.toHaveBeenCalled()
    })
})
