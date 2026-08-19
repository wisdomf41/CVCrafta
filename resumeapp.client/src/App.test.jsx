import { render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import App from './App'

vi.mock('./app/AppProviders', () => ({
    default: ({ children }) => (
        <div data-testid="app-providers">{children}</div>
    ),
}))

vi.mock('./app/AppRoutes', () => ({
    default: () => <section>Application routes</section>,
}))

vi.mock('./shared/components/AppNavbar', () => ({
    default: () => <nav>Application navigation</nav>,
}))

vi.mock('./shared/components/AppFooter', () => ({
    default: () => <footer>Application footer</footer>,
}))

// Verifies that App renders its navbar, route content, and footer inside AppProviders.
describe('App', () => {
    it('renders the complete application layout', () => {
        render(<App />)

        expect(screen.getByTestId('app-providers')).toBeInTheDocument()
        expect(screen.getByRole('navigation')).toBeInTheDocument()
        expect(screen.getByRole('main')).toHaveTextContent('Application routes')
        expect(screen.getByRole('contentinfo')).toBeInTheDocument()
    })
})
