# FullStack E-Commerce UI/UX Specification

## Part 1 — Product Foundation, Design System, Frontend Architecture, and Fixed Login

**Document:** `FullStack_ECommerce_UIUX.md`  
**Status:** Phase 1 UI/UX and frontend implementation specification  
**Primary source:** Existing full-stack e-commerce backend specification  
**Authentication mode for this phase:** Fixed demo login; replaceable later with Keycloak  
**Data policy:** Real business data from the ASP.NET Core API and SQL Server; no runtime mock business records  
**Frontend:** React + TypeScript + Vite  
**UI system:** Tailwind CSS + shadcn/ui + Radix UI primitives  
**Server state:** TanStack Query  
**Forms:** React Hook Form + Zod  
**Icons:** Lucide React  
**Charts:** Recharts  

---

## 1. Purpose and Scope

This document defines the frontend product experience and implementation rules for the e-commerce application described by the existing backend specification.

The application is an original marketplace-style storefront inspired by familiar large-scale commerce patterns.

The visual identity, copy, interaction patterns, component composition, and implementation details must remain original.

The application must not copy eBay logos, trademarks, proprietary layouts, copyrighted imagery, or proprietary content.

The experience should retain the useful characteristics of a high-volume marketplace:

- Search-first navigation.
- Broad category discovery.
- Dense but readable product cards.
- Strong filtering and sorting.
- Clear pricing and availability.
- Fast product comparison.
- Persistent cart access.
- Wishlist access.
- Straightforward checkout.
- Order visibility.
- Customer self-service.
- Separate administration workflows.

The first implementation phase intentionally uses a fixed demo login instead of Keycloak.

The fixed login exists only to unlock authenticated UI areas while the marketplace application is being built.

The fixed login must not change the underlying data architecture.

The application must still use real HTTP requests and persisted backend data for products, categories, carts, wishlists, addresses, orders, payments, refunds, reviews, inventory, and reports.

The fixed login layer must be isolated behind an authentication abstraction so it can later be replaced by Keycloak with minimal UI changes.

---

## 2. Phase 1 Authentication Decision

### 2.1 Fixed Login Is Temporary

Phase 1 uses a small frontend authentication adapter with fixed demo credentials.

The implementation is intended for local development and UX integration.

It is not a production security mechanism.

It must never be described as production authentication.

It must be easy to remove without rewriting feature modules.

The backend may also expose a development-only login endpoint if the implementation requires a backend session.

The preferred Phase 1 approach is to keep the authentication abstraction on the frontend and use the existing API directly for business data.

If the backend requires an identity header or development principal, that mechanism must be isolated in the API client.

No feature component should inspect hardcoded usernames or passwords.

No feature component should contain authentication rules.

### 2.2 Demo Accounts

The Phase 1 frontend may provide two demo identities.

| Username | Password | Display Name | Intended UI Area |
|---|---|---|---|
| `customer` | `customer123` | Demo Customer | Customer-facing marketplace and account UI |
| `admin` | `admin123` | Demo Administrator | Customer UI plus admin UI |

The exact credentials should live in one development-only configuration file.

They must not be scattered throughout components.

They must not be embedded in API functions.

They must not be displayed on public production builds.

Production builds must fail closed if the fixed-login feature is enabled accidentally.

### 2.3 Fixed Login Data Shape

Use a small internal type.

```ts
export type DemoRole = "customer" | "admin";

export interface AuthSession {
  isAuthenticated: boolean;
  username: string | null;
  displayName: string | null;
  role: DemoRole | null;
}
```

The session is UI state only during Phase 1.

The session must not be treated as authoritative authorization by the backend.

The UI may use the role to decide which navigation items to show in development.

The backend must still validate all real business mutations when production authentication is introduced.

### 2.4 Fixed Login Storage

Prefer in-memory session state during normal application execution.

A short-lived session marker may be stored in sessionStorage for local developer convenience.

Do not use localStorage as the source of truth for business data.

Do not store product records in localStorage.

Do not store cart totals in localStorage.

Do not store order status in localStorage.

Do not store payment status in localStorage.

Do not store inventory levels in localStorage.

The future Keycloak implementation should replace only the authentication adapter and provider.

Feature hooks, pages, and UI components should remain unchanged.

---

## 3. Source-of-Truth Rules

The backend specification requires real API and database data.

This document preserves that rule.

The frontend is never the authoritative source for business data.

The following must come from the API:

- Product IDs.
- Product names.
- Product descriptions.
- Product prices.
- Product stock quantities.
- Product activation status.
- Product categories.
- Product images when implemented.
- Product review summaries.
- Cart contents.
- Cart quantities.
- Cart totals.
- Wishlist contents.
- Customer profile data.
- Address records.
- Order numbers.
- Order status.
- Order totals.
- Payment status.
- Refund status.
- Inventory transactions.
- Sales statistics.
- Report metrics.
- Analytics values.

The frontend may hold transient UI state.

Examples of valid UI-only state include:

- Modal open or closed state.
- Selected tab.
- Expanded accordion.
- Search text before submission.
- Currently selected filters.
- Current pagination state before the request executes.
- Temporary form input.
- Hover and focus state.
- Theme preference.
- Table column visibility preference.

The frontend must not invent business state.

The frontend must not generate business IDs.

The frontend must not generate order totals as authoritative values.

The frontend must not generate payment outcomes.

The frontend must not decrement inventory directly.

The frontend must not create fake KPIs.

---

## 4. Product Experience Goals

The product should feel like a modern, trustworthy marketplace.

The main experience should be discoverable within seconds.

The primary user journey is:

```text
Open storefront
    -> Discover categories or search
    -> Search or browse products
    -> Filter and sort
    -> Open product details
    -> Add to wishlist or cart
    -> Review cart
    -> Checkout
    -> Complete development payment
    -> View order
```

The customer should always understand:

- Where they are.
- What they are looking at.
- What they can do next.
- Whether an action succeeded.
- Whether an action failed.
- Whether data is loading.
- Whether there are no results.
- Whether an item is unavailable.

The admin should always understand:

- Which module is open.
- Which records are currently displayed.
- Which filters are active.
- Which records are selected.
- What action is available.
- What action has already completed.
- Whether metrics are real and current.

---

## 5. UX Principles

### 5.1 Search First

Search is a primary navigation mechanism.

The global search field is visible on desktop.

The global search field is easily reachable on mobile.

Search suggestions should reduce typing without replacing the real product search API.

Search autocomplete must use API results or derived values from already loaded real data.

Search suggestions must never use a hardcoded fake product list.

### 5.2 Progressive Disclosure

Do not show every possible control at once.

Show the most important action first.

Secondary actions may live in menus or overflow controls.

Desktop may expose filters in a side rail.

Tablet may use a collapsible filter panel.

Mobile should use a filter drawer or sheet.

### 5.3 Clear Hierarchy

Use typography, spacing, and grouping to establish visual hierarchy.

The most important information on a product card is:

1. Product image.
2. Product name.
3. Price.
4. Availability.
5. Primary action.

The most important information on an order card is:

1. Order number.
2. Order date.
3. Current status.
4. Total.
5. Primary next action.

### 5.4 Honest Feedback

Every async action must expose a visible result.

Examples:

- Loading.
- Success.
- Validation failure.
- Business conflict.
- Unauthorized.
- Not found.
- Rate limited.
- Server error.

Avoid silent failures.

Avoid success messages for requests that have not completed.

Avoid optimistic updates when the server result materially determines correctness unless rollback is reliable.

### 5.5 Empty Data Is a Valid State

An empty database is not an error.

An empty search result is not a system failure.

An empty wishlist is not an error.

An empty order history is not an error.

The UI should explain what is empty and provide an appropriate next action.

The UI must never insert sample records to make an empty state look populated.

---

## 6. Original Brand Direction

The application should use an original commerce brand.

Recommended working name for the UI specification:

**MarketNest**

The brand name is a placeholder for the implementation team and may be replaced later.

The interface should communicate:

- Reliability.
- Variety.
- Convenience.
- Speed.
- Transparency.

The visual identity should use a clean neutral base with one strong primary accent and one supporting accent.

Avoid copying the familiar multicolor letter treatment associated with major marketplace brands.

Avoid using their trademark color combinations as a brand signature.

Avoid recognizable competitor logos.

Use original icon combinations and illustrations.

### 6.1 Brand Voice

Brand copy should be:

- Clear.
- Concise.
- Helpful.
- Neutral.
- Action-oriented.
- Non-pushy.

Avoid exaggerated claims unless supported by real backend data.

Do not use statements such as “millions sold” unless a real API statistic supports the statement.

Do not display “best seller” labels unless a backend rule provides that classification.

Do not display fake discount percentages.

Do not display fake review counts.

---

## 7. Visual Design System

### 7.1 Design Language

Use a high-information marketplace interface with generous whitespace around major sections.

The design should feel dense in data areas and spacious in navigation areas.

The storefront should prioritize scanning.

The admin interface should prioritize operational clarity.

Use neutral surfaces for most content.

Reserve accent colors for actions, active states, links, and meaningful status indicators.

### 7.2 Base Visual Tokens

```text
Base background: near-white neutral
Surface: white
Surface muted: light neutral
Border: subtle neutral
Text primary: deep neutral
Text secondary: muted neutral
Text disabled: low-contrast neutral
Primary action: strong accent
Primary action hover: darker accent
Secondary action: neutral outline
Success: semantic green family
Warning: semantic amber family
Danger: semantic red family
Info: semantic blue family
```

Exact token values should be defined in Tailwind theme variables.

Do not scatter raw color values throughout components.

### 7.3 Color Token Naming

Use semantic names.

```text
--background
--foreground
--card
--card-foreground
--popover
--popover-foreground
--primary
--primary-foreground
--secondary
--secondary-foreground
--muted
--muted-foreground
--accent
--accent-foreground
--destructive
--destructive-foreground
--border
--input
--ring
```

Add application-specific semantic tokens for:

```text
--success
--success-foreground
--warning
--warning-foreground
--info
--info-foreground
--price
--stock-in
--stock-low
--stock-out
```

Use semantic tokens instead of hardcoding utility combinations in every component.

### 7.4 Contrast Rules

Text must maintain accessible contrast.

Interactive controls must have visible focus states.

Disabled controls may have reduced contrast but should remain understandable.

Do not communicate meaning using color alone.

Status badges should combine color with text.

Stock indicators should combine color with labels.

Validation states should include text or icons with accessible labels.

---

## 8. Typography System

Use a modern sans-serif system font stack by default.

Recommended stack:

```css
font-family:
  Inter,
  ui-sans-serif,
  system-ui,
  -apple-system,
  BlinkMacSystemFont,
  "Segoe UI",
  sans-serif;
```

If a branded font is added later, use it only for major headings and brand surfaces.

Do not use more than two font families.

### 8.1 Type Scale

```text
Display: 48px / 56px / 700
Heading XL: 36px / 44px / 700
Heading LG: 30px / 38px / 700
Heading MD: 24px / 32px / 700
Heading SM: 20px / 28px / 600
Body LG: 18px / 28px / 400
Body: 16px / 24px / 400
Body SM: 14px / 20px / 400
Caption: 12px / 16px / 500
```

Use responsive type scaling where appropriate.

Do not use very small body text for critical product or checkout information.

### 8.2 Product Price Typography

Primary product prices should be visually prominent.

Use a strong weight.

Keep currency and amount together.

Use consistent decimal formatting based on the API currency contract.

Do not assume a currency in the UI if the backend contract is not finalized.

Centralize currency formatting in a shared component.

### 8.3 Text Truncation

Product names may be truncated on compact cards.

Never truncate the primary product title on the detail page.

Use tooltips only when truncated text is difficult to understand.

Prefer line clamping to hidden overflow when it improves scanning.

---

## 9. Spacing System

Use a consistent spacing scale.

Preferred base unit: 4px.

Recommended spacing tokens:

```text
0: 0px
1: 4px
2: 8px
3: 12px
4: 16px
5: 20px
6: 24px
8: 32px
10: 40px
12: 48px
16: 64px
20: 80px
24: 96px
```

Prefer Tailwind spacing utilities.

Avoid arbitrary values unless the design requires a unique measurement.

Use consistent vertical rhythm between section headings and content.

### 9.1 Content Spacing

Page shell padding:

- Desktop: 24px to 40px.
- Tablet: 20px to 24px.
- Mobile: 16px.

Section spacing:

- Desktop: 48px to 72px.
- Tablet: 40px to 56px.
- Mobile: 32px to 40px.

Card internal padding:

- Standard: 16px.
- Compact: 12px.
- Large feature card: 20px to 24px.

---

## 10. Layout Grid

Use a centered max-width content container.

Recommended maximum width:

```text
max-width: 1440px
```

The desktop storefront should support:

- Full-width header.
- Centered content shell.
- Optional wide promotional areas.
- Product grid from 4 to 6 columns depending on viewport.

Suggested layout widths:

```text
Sidebar: 240px to 280px
Content gap: 24px
Main content: remaining width
```

On product listing pages:

```text
Desktop:
[Filter rail][Product grid]

Tablet:
[Filter button][Product grid]

Mobile:
[Filter button][Sort button]
[Product grid]
```

Do not force desktop sidebars onto narrow screens.

---

## 11. Responsive Breakpoints

Use Tailwind breakpoints unless the implementation team has a strong reason to customize them.

Recommended behavior:

```text
sm: 640px
md: 768px
lg: 1024px
xl: 1280px
2xl: 1536px
```

### 11.1 Mobile

Primary concerns:

- Thumb-friendly controls.
- Minimal header height.
- Full-width search.
- Drawer-based navigation.
- One-column product cards.
- Bottom-accessible primary actions.

### 11.2 Tablet

Primary concerns:

- Two-column or three-column product grids.
- Collapsible filters.
- Condensed utility navigation.
- Search remains prominent.

### 11.3 Desktop

Primary concerns:

- Full navigation.
- Persistent category access.
- Multi-column product grids.
- Side filter rail.
- Dense admin tables.

---

## 12. Tailwind CSS Standards

Tailwind CSS is the default styling mechanism.

Prefer utility classes composed through reusable components.

Do not create large global CSS files for component-specific layout.

Use CSS variables for theme tokens.

Use `cn()` for conditional class composition.

Use `cva` where variants become complex.

### 12.1 Utility Composition Rules

Prefer semantic component variants over repeated utility strings.

Example:

```ts
const buttonVariants = cva(
  "inline-flex items-center justify-center rounded-md font-medium transition-colors focus-visible:outline-none focus-visible:ring-2",
  {
    variants: {
      variant: {
        primary: "bg-primary text-primary-foreground hover:bg-primary/90",
        secondary: "border bg-background hover:bg-muted",
        destructive: "bg-destructive text-destructive-foreground hover:bg-destructive/90",
      },
      size: {
        sm: "h-9 px-3 text-sm",
        md: "h-10 px-4",
        lg: "h-11 px-6",
      },
    },
    defaultVariants: {
      variant: "primary",
      size: "md",
    },
  },
);
```

### 12.2 Avoid Arbitrary CSS Everywhere

Avoid patterns such as:

```text
text-[#123456]
p-[17px]
mt-[13px]
```

Use design tokens and standard spacing whenever possible.

Arbitrary values are acceptable only for documented exceptions.

### 12.3 Layout Rules

Prefer CSS Grid for page-level structure.

Prefer Flexbox for linear component layout.

Avoid deeply nested wrappers that provide no layout purpose.

Use `gap` instead of margin chains for repeated layouts.

Keep content widths predictable.

---

## 13. Recommended Tailwind Theme Structure

The Tailwind theme should define semantic color variables and spacing behavior.

Suggested conceptual configuration:

```ts
export default {
  theme: {
    extend: {
      colors: {
        border: "hsl(var(--border))",
        background: "hsl(var(--background))",
        foreground: "hsl(var(--foreground))",
        primary: {
          DEFAULT: "hsl(var(--primary))",
          foreground: "hsl(var(--primary-foreground))",
        },
        secondary: {
          DEFAULT: "hsl(var(--secondary))",
          foreground: "hsl(var(--secondary-foreground))",
        },
        destructive: {
          DEFAULT: "hsl(var(--destructive))",
          foreground: "hsl(var(--destructive-foreground))",
        },
        muted: {
          DEFAULT: "hsl(var(--muted))",
          foreground: "hsl(var(--muted-foreground))",
        },
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
    },
  },
};
```

The exact configuration can follow the installed Tailwind version.

The implementation must not blindly copy this example if the installed Tailwind version uses a different configuration mechanism.

---

## 14. Border Radius System

Use rounded corners consistently.

Recommended:

```text
sm: 6px
md: 8px
lg: 12px
xl: 16px
2xl: 20px
pill: 9999px
```

Use smaller radius for:

- Inputs.
- Buttons.
- Compact controls.

Use medium radius for:

- Cards.
- Dropdowns.
- Dialogs.

Use larger radius for:

- Hero sections.
- Large promotional surfaces.
- Mobile bottom sheets.

Avoid excessive pill styling for every control.

Pills should be reserved for chips, tags, and compact statuses.

---

## 15. Elevation and Shadows

Use subtle shadows.

Preferred hierarchy:

```text
Level 0: no shadow
Level 1: subtle card elevation
Level 2: dropdown and popover elevation
Level 3: dialog and drawer elevation
Level 4: floating navigation or high-priority overlays
```

Avoid heavy shadows on every card.

Use borders to separate dense data surfaces.

Use elevation to establish layering, not decoration.

---

## 16. shadcn/ui Component Standards

Use shadcn/ui components as the base for accessible primitives.

Recommended components:

- Button.
- Input.
- Label.
- Textarea.
- Select.
- Checkbox.
- RadioGroup.
- Switch.
- Dialog.
- Sheet.
- Drawer.
- DropdownMenu.
- Popover.
- Command.
- Tooltip.
- Tabs.
- Accordion.
- Card.
- Badge.
- Alert.
- Skeleton.
- Separator.
- Breadcrumb.
- Pagination.
- Table.
- Calendar.
- Form.

Do not duplicate primitives in feature folders.

The `components/ui` directory should contain shared presentation primitives.

Feature-specific wrappers belong in feature folders.

Example:

```text
components/ui/Button.tsx
features/products/components/ProductCard.tsx
features/products/components/ProductFilters.tsx
```

Do not create a second Button implementation inside `features/products`.

---

## 17. Component Variant Rules

Component variants must communicate purpose.

Avoid arbitrary combinations of classes in pages.

Use named variants.

Examples:

```text
Button: primary | secondary | outline | ghost | destructive
Badge: default | success | warning | danger | info
Card: default | elevated | interactive | compact
StatusBadge: pending | confirmed | shipped | delivered | cancelled
```

The same semantic status must have the same visual treatment across the product.

A `Delivered` status should not appear green in one page and blue in another.

---

## 18. Lucide Icon Standards

Use Lucide React for interface icons.

Do not use emoji as functional interface icons.

Icons must be visually consistent in size.

Recommended icon sizes:

```text
12px: very compact metadata
14px: dense table controls
16px: standard controls
18px: prominent buttons
20px: header actions
24px: mobile navigation
```

Icon-only buttons must have accessible names.

Use `aria-label` when visible text is absent.

Avoid using icons as the only indicator for destructive actions.

---

## 19. Motion System

Animations must reinforce hierarchy and feedback.

Recommended durations:

```text
Micro interaction: 100ms to 150ms
Standard transition: 150ms to 250ms
Panel transition: 200ms to 300ms
Page-level motion: 250ms to 400ms
```

Use easing that feels natural.

Avoid continuous decorative motion.

Avoid bouncing buttons.

Avoid animations that delay checkout or critical actions.

### 19.1 Recommended Motion

Use subtle animation for:

- Search dropdown appearance.
- Dialog entry.
- Sheet entry.
- Product card hover elevation.
- Wishlist toggle feedback.
- Cart item removal.
- Toast appearance.
- Skeleton shimmer.

### 19.2 Reduced Motion

Respect `prefers-reduced-motion`.

Disable nonessential motion when the user requests reduced motion.

Do not rely on motion to communicate status.

---

## 20. Accessibility Requirements

Target WCAG 2.2 AA for the interface.

### 20.1 Keyboard Navigation

Every interactive feature must be keyboard accessible.

Keyboard focus must be visible.

Dialogs must trap focus appropriately.

Dropdowns must support keyboard navigation.

Search autocomplete must support arrow-key selection.

Escape should close dismissible overlays.

### 20.2 Screen Reader Support

Use semantic HTML.

Prefer `button` for actions.

Prefer `a` for navigation.

Do not use clickable `div` elements for primary interactions.

Provide labels for inputs.

Use `aria-live` sparingly for async feedback.

Use status regions for save success and failure messages where appropriate.

### 20.3 Forms

Every form field must have a visible or programmatically associated label.

Validation errors must identify the field.

Error text must not depend only on color.

Use `aria-describedby` for detailed field guidance and errors.

### 20.4 Tables

Admin tables must:

- Use semantic table markup.
- Provide column headers.
- Support keyboard navigation when interactive.
- Avoid extremely dense touch targets on mobile.

Mobile tables may become card lists when a tabular layout is no longer practical.

---

## 21. React + TypeScript Architecture

Use feature-based architecture.

Avoid organizing the application only by technical type.

Business features should own their API functions, query keys, hooks, components, schemas, and types.

Recommended structure:

```text
src/
├── app/
│   ├── providers/
│   ├── router/
│   └── App.tsx
├── components/
│   ├── ui/
│   ├── common/
│   └── feedback/
├── core/
│   ├── api/
│   ├── auth/
│   ├── config/
│   ├── constants/
│   ├── types/
│   ├── utils/
│   └── validation/
├── features/
│   ├── categories/
│   ├── products/
│   ├── search/
│   ├── cart/
│   ├── wishlist/
│   ├── checkout/
│   ├── orders/
│   ├── payments/
│   ├── refunds/
│   ├── reviews/
│   ├── profile/
│   ├── addresses/
│   ├── inventory/
│   ├── users/
│   └── reports/
├── layouts/
│   ├── StorefrontLayout.tsx
│   ├── AccountLayout.tsx
│   └── AdminLayout.tsx
├── pages/
│   ├── public/
│   ├── account/
│   └── admin/
├── lib/
│   └── utils.ts
└── main.tsx
```

The final project should also follow the backend specification's monorepo structure.

The frontend remains separate from the ASP.NET Core solution.

---

## 22. Feature Folder Contract

Every mature feature should follow a predictable structure.

Example:

```text
features/products/
├── api/
│   ├── productApi.ts
│   └── productKeys.ts
├── components/
│   ├── ProductCard.tsx
│   ├── ProductGrid.tsx
│   ├── ProductFilters.tsx
│   └── ProductSort.tsx
├── hooks/
│   ├── useProductSearch.ts
│   └── useProduct.ts
├── schemas/
│   └── productSchemas.ts
├── types/
│   └── product.types.ts
└── utils/
    └── productFormatters.ts
```

Rules:

- API functions belong in `api`.
- TanStack Query hooks belong in `hooks`.
- Zod schemas belong in `schemas`.
- DTO and UI model types belong in `types`.
- Reusable feature components belong in `components`.
- Feature-specific formatting logic belongs in `utils`.

Do not create a `mocks` directory.

Do not create runtime fixtures.

---

## 23. Core API Layer

React components must never call `fetch` or Axios directly.

The request flow must be:

```text
Page
  -> Feature Hook
  -> Feature API Function
  -> Core API Client
  -> ASP.NET Core API
```

Example:

```ts
export const productKeys = {
  all: ["products"] as const,
  search: (params: ProductSearchParams) =>
    ["products", "search", params] as const,
  detail: (id: string) =>
    ["products", "detail", id] as const,
};
```

The API client should own:

- Base URL.
- Request headers.
- Development authentication context.
- JSON serialization.
- Response parsing.
- Error normalization.
- Request cancellation.
- Correlation ID handling if supported.

The API client should not contain business-specific transformation logic.

Feature API functions should map backend endpoints to typed frontend contracts.

---

## 24. Typed API Contract Rules

All API functions should have explicit return types.

Avoid `any`.

Use generated OpenAPI types where practical.

If code generation is not enabled, define local DTO types that accurately match the backend.

Do not silently change backend enum names in API types.

If a UI label differs from a backend enum, transform it in a presentation helper.

Example:

```ts
export type OrderStatus =
  | "Pending"
  | "Confirmed"
  | "Packed"
  | "Shipped"
  | "Delivered"
  | "Cancelled"
  | "PendingPayment"
  | "PaymentFailed";
```

UI labels should be mapped centrally.

Never duplicate status mappings across pages.

---

## 25. TanStack Query Strategy

TanStack Query is the source of client-side server-state caching.

It is not the source of truth.

The backend remains authoritative.

Use query keys that encode all relevant server parameters.

Example:

```ts
["products", "search", {
  keyword,
  categoryId,
  page,
  pageSize,
}]
```

After mutations:

- Invalidate affected queries.
- Update cached data only when the server response is authoritative.
- Avoid manually reconstructing complex business totals.

Examples:

Adding an item to cart should invalidate the cart query.

Changing cart quantity should invalidate or update the cart query with the returned response.

Creating a product should invalidate the product list and any relevant category queries.

Updating inventory should invalidate inventory history and any affected product availability query.

### 25.1 Query Defaults

Use conservative defaults.

Do not poll every endpoint continuously.

Use refetch-on-focus selectively.

Use stale times based on expected volatility.

Suggested direction:

```text
Categories: relatively long stale time
Product details: moderate stale time
Product search: moderate stale time
Cart: short stale time
Orders: short to moderate stale time
Inventory: short stale time
Reports: controlled refresh
```

Exact values should be tuned after observing API behavior.

---

## 26. API Error Normalization

Normalize backend failures into one frontend error shape.

Example:

```ts
export interface ApiError {
  status: number;
  title?: string;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}
```

Map status codes consistently.

```text
400 -> validation or malformed request
401 -> fixed-login session unavailable in Phase 1 or future authentication failure
403 -> forbidden state when backend authorization is active
404 -> not found state
409 -> business conflict
429 -> rate limit state
500 -> server error
503 -> service unavailable
```

During Phase 1 fixed login, `401` should be handled as an authentication/session problem.

The UI must not claim “you do not have permission” unless the backend returned a relevant authorization response.

---

## 27. Loading State System

Every API-driven screen must have a loading state.

Prefer skeletons for predictable content areas.

Use spinners for compact actions.

Avoid replacing the entire page with a spinner after every small mutation.

### 27.1 Product Loading

Product cards should use image, title, price, and metadata skeletons.

### 27.2 Table Loading

Tables should render a skeleton row structure matching the expected columns.

### 27.3 Detail Loading

Use image gallery skeletons and content placeholders.

### 27.4 Mutation Loading

Buttons performing mutations should show loading text or a spinner.

Disable duplicate submission while a mutation is pending.

---

## 28. Empty State System

All empty states should include:

- Clear heading.
- One-sentence explanation.
- Appropriate visual icon or illustration.
- Next action when available.

Examples:

```text
No products found
Try a different keyword or remove a filter.
[Clear filters]
```

```text
Your wishlist is empty
Save products here to compare or revisit them later.
[Browse products]
```

```text
No orders yet
Completed purchases will appear here.
[Start shopping]
```

Never include fake products in empty states.

---

## 29. Global Application Layout

The storefront layout should contain:

```text
StorefrontLayout
├── UtilityBar
├── MainHeader
│   ├── BrandLogo
│   ├── CategoryMenu
│   ├── SearchBar
│   ├── WishlistAction
│   ├── AccountAction
│   └── CartAction
├── CategoryNav
├── MainContent
└── Footer
```

The admin layout should be separate.

The account layout should be separate.

Shared primitives may be reused.

Page-specific navigation should not leak into the global storefront header.

---

## 30. Desktop Header Specification

Desktop header has three conceptual layers.

### 30.1 Utility Row

Contains:

- Welcome text or sign-in state.
- Help link.
- Customer account shortcut.
- Wishlist shortcut.
- Admin shortcut when fixed demo role is admin.

Keep the row visually quiet.

### 30.2 Main Navigation Row

Contains:

- Brand.
- Shop by category control.
- Large search input.
- Category selector.
- Search button.
- Wishlist icon.
- Cart icon.

The search bar should occupy the largest horizontal area.

The cart count must come from the real cart query for authenticated demo sessions.

If the cart query is unavailable, show an explicit loading or unavailable state rather than an invented count.

### 30.3 Category Row

Display active categories loaded from the API.

Do not hardcode category names.

Category navigation should support horizontal scrolling when the number of categories exceeds available width.

---

## 31. Tablet Header Specification

Tablet layouts should simplify the header.

Use:

- Brand.
- Search.
- Cart.
- Menu trigger.

Place account and wishlist actions inside the menu when horizontal space is limited.

The category navigation may become horizontally scrollable.

Avoid wrapping the main header into multiple unpredictable lines.

---

## 32. Mobile Header Specification

Mobile header should contain:

```text
[Menu] [Brand] [Cart]
[Search field]
```

The search field should remain easy to access.

A category drawer should contain API-loaded categories.

The mobile menu should include:

- Home.
- Shop by category.
- Products.
- Wishlist.
- Cart.
- Account.
- Admin area for the demo admin user.

The fixed-login role should not be exposed as a security guarantee.

The menu is only a Phase 1 UI gate.

---

## 33. Search Bar Specification

The search bar is a global component.

Props should include:

```ts
interface SearchBarProps {
  value: string;
  onChange: (value: string) => void;
  onSubmit: () => void;
  placeholder?: string;
  disabled?: boolean;
}
```

Search submission should navigate to the product listing route with query parameters.

Example:

```text
/products?keyword=wireless+headphones&page=1&pageSize=24
```

The frontend should not execute local filtering across a large product dataset.

The backend search API remains authoritative.

---

## 34. Search Autocomplete UX

Autocomplete should appear after the user enters a meaningful query.

Use a small debounce.

Do not call the API on every keystroke without a debounce strategy.

Suggestions may include:

- Matching product names.
- Matching categories.
- Relevant recent search terms if stored as UI preference.

Product suggestions must come from real API data.

Do not fabricate suggestions.

### 34.1 Keyboard Behavior

Support:

- Arrow down.
- Arrow up.
- Enter.
- Escape.

The active suggestion must be visually distinct.

The component should use the appropriate combobox semantics.

### 34.2 Mobile Behavior

On mobile, autocomplete may open in a full-width sheet or inline panel.

Ensure the keyboard does not obscure the most relevant suggestions.

### 34.3 Search Result Navigation

Selecting a product suggestion should navigate directly to its real product detail route.

Selecting a category should navigate to the category listing route.

Submitting arbitrary text should navigate to the product search route.

---

## 35. Category Navigation

Categories are retrieved from the category API.

The navigation should support:

- Top categories.
- Category listing.
- Category detail.
- Category product count when supported by the backend.

Do not display product counts unless the API returns real counts.

Do not invent category hierarchy if the backend does not expose a hierarchy.

If hierarchy is later added, use a recursive navigation component.

---

## 36. Breadcrumb Standards

Use breadcrumbs on:

- Product details.
- Category pages.
- Account pages.
- Admin detail pages.

Example:

```text
Home / Electronics / Audio / Product Name
```

Breadcrumb items should use real category and product names.

Do not hardcode breadcrumb content.

The current page should not be an interactive link unless there is a clear navigational reason.

---

## 37. Footer Specification

The storefront footer should contain:

- About the marketplace.
- Customer service.
- Shopping help.
- Account links.
- Legal links.
- Contact information when provided by the application configuration.

Do not invent physical addresses, phone numbers, or support hours.

Footer links should either navigate to real pages or remain clearly marked as future content during development.

The footer should be responsive.

On mobile, use accordion sections.

On desktop, use multi-column groups.

---

## 38. Fixed Login Page

The fixed login page is a temporary Phase 1 experience.

Route:

```text
/login
```

The page should contain:

- Brand logo.
- Welcome heading.
- Username input.
- Password input.
- Sign-in button.
- Demo account hint in development builds only.
- Error message for invalid credentials.
- Link back to storefront.

The login form must use React Hook Form and Zod.

Validation:

- Username required.
- Password required.

Authentication flow:

```text
User submits login
    -> Validate form
    -> Check fixed development credentials
    -> Create AuthSession
    -> Store session in auth provider
    -> Navigate to intended route or home
```

### 38.1 Login Success

Show a brief success transition only if it does not delay navigation.

Navigate directly to the intended destination.

### 38.2 Login Failure

Use a generic error:

```text
Invalid username or password.
```

Do not reveal which credential is incorrect.

### 38.3 Logout

Logout should:

- Clear the session.
- Clear user-specific TanStack Query cache where appropriate.
- Navigate to the storefront.

Do not leave customer-specific cart or wishlist data visible after logout.

---

## 39. Fixed Login Architecture

Use an interface that can later be replaced.

```ts
export interface AuthAdapter {
  login(username: string, password: string): Promise<AuthSession>;
  logout(): Promise<void>;
  getSession(): AuthSession;
  isAuthenticated(): boolean;
}
```

Phase 1 implementation:

```ts
export class FixedDemoAuthAdapter implements AuthAdapter {
  // development-only implementation
}
```

Future implementation:

```ts
export class KeycloakAuthAdapter implements AuthAdapter {
  // later production authentication implementation
}
```

The rest of the application should depend on `AuthAdapter` or a React hook rather than the concrete class.

---

## 40. Auth Provider

The React provider should expose:

```ts
interface AuthContextValue {
  session: AuthSession;
  isLoading: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}
```

The provider should be mounted near the root application provider tree.

Recommended order:

```text
BrowserRouter
  -> QueryClientProvider
  -> AuthProvider
  -> AppRouter
```

The exact provider nesting may vary.

Avoid coupling the AuthProvider to feature APIs during Phase 1.

When Keycloak is added later, the provider can wrap the Keycloak adapter.

---

## 41. Route Strategy for Phase 1

Public routes:

```text
/
/products
/products/:productId
/categories/:categoryId
/login
```

Customer routes:

```text
/cart
/checkout
/payment/:paymentId
/payment/:paymentId/result
/account
/account/profile
/account/addresses
/account/wishlist
/account/orders
/account/orders/:orderId
/account/refunds
```

Admin routes:

```text
/admin
/admin/categories
/admin/products
/admin/inventory
/admin/orders
/admin/payments
/admin/refunds
/admin/reviews
/admin/customers
/admin/reports
```

Phase 1 route guards may use the fixed demo role.

The implementation should make route guards replaceable later.

---

## 42. Route Guard Abstraction

Use a generic guard.

```ts
interface RouteGuardProps {
  children: React.ReactNode;
  requireAuth?: boolean;
  requiredRole?: DemoRole;
}
```

Phase 1 behavior:

- `requireAuth=true` requires a fixed login session.
- `requiredRole="admin"` requires the demo admin session.

Future behavior:

- Authentication is determined by Keycloak.
- Roles are determined by validated token claims and backend policies.

The rest of the route configuration should not need structural changes.

---

## 43. Account Navigation

The account area should use a consistent navigation panel.

Sections:

- Overview.
- Profile.
- Addresses.
- Wishlist.
- Orders.
- Refunds.

Mobile behavior:

- Horizontal tab scrolling or dropdown navigation.

Desktop behavior:

- Left-side navigation panel.
- Main content panel.

The account layout must remain usable with an empty order history.

---

## 44. Admin Navigation Foundation

The admin layout should contain:

```text
AdminLayout
├── Sidebar
│   ├── Dashboard
│   ├── Categories
│   ├── Products
│   ├── Inventory
│   ├── Orders
│   ├── Payments
│   ├── Refunds
│   ├── Reviews
│   ├── Customers
│   └── Reports
├── Header
│   ├── Breadcrumbs
│   ├── Search or quick action area
│   └── Current user menu
└── MainContent
```

The admin sidebar should collapse on desktop.

On mobile, use a sheet or drawer.

The admin dashboard must not display hardcoded metrics.

A zero metric is valid.

An empty chart is valid.

A missing report endpoint should produce an explicit unavailable state rather than fake data.

---

## 45. Component Hierarchy Rules

Global components should remain low-level and reusable.

Feature components should express business concepts.

Pages should compose feature components.

Example:

```text
HomePage
├── HeroSearchSection
│   └── SearchBar
├── CategoryDiscovery
│   └── CategoryCard[]
├── RecentProductsSection
│   └── ProductGrid
│       └── ProductCard[]
└── EmptyOrFallbackSections
```

Product listing:

```text
ProductListingPage
├── PageHeader
├── SearchSummary
├── FilterToolbar
├── ProductResults
│   ├── LoadingSkeleton
│   ├── EmptyState
│   ├── ErrorState
│   └── ProductGrid
└── Pagination
```

Admin dashboard:

```text
AdminDashboardPage
├── PageHeader
├── KPIGrid
├── SalesOverviewChart
├── OrderStatusChart
├── LowStockTable
└── RecentOrdersTable
```

Every visual data point must be backed by real API data.

---

## 46. Common Feedback Components

Implement these reusable components early:

```text
LoadingSkeleton
ErrorState
EmptyState
NotFoundState
UnauthorizedState
InlineFieldError
ToastProvider
ConfirmDialog
StatusBadge
```

Each should have a consistent visual pattern.

### 46.1 ErrorState

Include:

- Clear title.
- Short explanation.
- Retry action when retry is meaningful.

### 46.2 EmptyState

Include:

- Clear empty heading.
- Helpful explanation.
- Relevant action.

### 46.3 ConfirmDialog

Use for:

- Remove cart item.
- Delete address.
- Remove wishlist item when a confirmation is appropriate.
- Admin destructive actions.

Do not use confirmation dialogs for every minor action.

---

## 47. Confirmation and Destructive Action Rules

Destructive actions must be visually clear.

Use a destructive variant for irreversible or potentially irreversible operations.

Examples:

- Delete category.
- Delete product.
- Delete address.
- Remove cart item.
- Reject refund.

The dialog should explain the consequence.

Do not use vague labels such as `OK`.

Prefer:

```text
Delete product
Cancel
```

Avoid destructive actions as the default primary button when a safer alternative exists.

---

## 48. Toast Notification Standards

Toasts are for lightweight feedback.

Use them for:

- Wishlist item added.
- Item removed.
- Cart item added.
- Profile saved.
- Address saved.
- Admin update completed.

Do not use toasts as the only validation mechanism.

Do not use toasts for critical payment errors that require user attention on the page.

Toasts should have accessible live-region behavior.

---

## 49. Data Freshness Indicators

For dashboards and reports, optionally show:

```text
Last updated: 2 minutes ago
```

Only when the API provides a meaningful timestamp or the frontend can safely identify the last successful fetch time.

Do not imply real-time data if the UI merely cached a previous response.

Refresh actions must trigger actual API refetches.

---

## 50. Performance Rules

Performance must support real API usage.

Use pagination for large collections.

Do not load all products to filter locally.

Use image lazy loading where appropriate.

Use responsive image sizes when supported.

Avoid rendering very large product grids without virtualization or pagination.

Avoid unnecessary query refetches.

Use route-level lazy loading for heavy admin pages.

Do not block first content render on optional analytics widgets.

### 50.1 Code Splitting

Recommended route groups:

```text
Public storefront bundle
Account bundle
Admin bundle
```

The admin bundle may be lazy-loaded.

The reports module may be lazy-loaded because charting libraries can be large.

---

## 51. Image and Media Rules

Use real image URLs returned by the backend.

Provide alt text based on product names and meaningful context.

Do not embed base64 product images in product DTOs.

When an image is unavailable, display a consistent neutral placeholder.

Do not generate fake product photography inside the frontend.

Do not use random image URLs from external services as runtime business content.

Use object-fit rules based on the image type.

Product thumbnails should generally use `object-contain` when the product itself should remain fully visible.

Lifestyle images may use `object-cover` when the backend supports them.

---

## 52. Product Card Foundation

The common product card must be reusable across:

- Home page.
- Search results.
- Category pages.
- Wishlist.
- Related products.
- Recently viewed sections.

The card may display:

- Product image.
- Product name.
- Price.
- Stock state.
- Rating summary when available.
- Wishlist control.
- Add-to-cart action.

Do not display:

- Random star ratings.
- Fake review counts.
- Fake sales counts.
- Fake discounts.
- Invented shipping promises.

### 52.1 Product Card Interaction

Clicking the product body navigates to the detail page.

Clicking the wishlist button must not navigate to the product detail page.

Clicking Add to Cart must not navigate unless the UX explicitly chooses a cart transition.

Use event propagation control carefully.

---

## 53. Product Listing Foundations

The product listing screen must support the backend's initial query contract.

Current baseline parameters:

```text
keyword
categoryId
page
pageSize
```

The UI should be architected to add later:

```text
minPrice
maxPrice
inStock
condition
brand
sortBy
sortDirection
```

The page must not expose controls that the backend does not support yet.

Controls should become visible only when corresponding API support exists.

---

## 54. Pagination Standards

Use server-side pagination.

Do not load every product and paginate in the browser.

Display:

- Current page.
- Total pages when returned.
- Previous.
- Next.
- Optional page-size selector.

On mobile, simplify the pagination control.

Example:

```text
[Previous]  Page 2 of 10  [Next]
```

All page changes should update the URL query parameters.

This enables shareable and bookmarkable search results.

---

## 55. URL State Rules

Use the URL for durable page state that should survive refresh.

Recommended URL state:

- Search keyword.
- Category.
- Page.
- Page size.
- Sort.
- Filters.

Use local component state for temporary UI state.

Do not put modal open state in the URL unless deep-linking is required.

Keep query parameter names consistent across pages.

---

## 56. Form Architecture

Use React Hook Form for complex forms.

Use Zod for client-side validation.

Client-side validation improves user experience.

It does not replace backend validation.

The server remains authoritative.

### 56.1 Form Submission Flow

```text
User input
    -> React Hook Form
    -> Zod validation
    -> API request
    -> Backend validation
    -> Success or normalized error
    -> UI feedback
```

Map backend validation errors to the correct fields when possible.

Show a generic form error when the backend returns a non-field error.

---

## 57. Mobile Touch Target Rules

Interactive controls should have comfortable touch targets.

Avoid tiny icon buttons.

Use larger hit areas than the visible icon itself.

Do not rely on hover for important information on touch devices.

Admin table actions should move into menus on narrow screens when necessary.

---

## 58. Dark Mode Strategy

Dark mode is optional for Phase 1.

Light mode is the primary supported theme.

If dark mode is implemented:

- Use semantic tokens.
- Do not hardcode light-only colors.
- Ensure charts remain readable.
- Ensure status colors remain distinguishable.
- Test image and card surfaces.

The theme preference may be stored as UI preference.

Theme preference is not business data.

---

## 59. AI Implementation Rules

AI-assisted development must follow the project's architecture.

AI-generated code must not introduce mock business data.

AI-generated code must not create fake API functions.

AI-generated code must not hardcode product records.

AI-generated code must not invent backend endpoints.

AI should first inspect the existing API contract.

AI should preserve existing domain and application boundaries.

AI should extend existing modules rather than replace them.

### 59.1 AI Generation Order

The recommended order for AI-assisted implementation is:

```text
1. Read backend endpoint contracts.
2. Identify existing DTOs.
3. Identify missing UI-required fields.
4. Create typed frontend DTOs.
5. Create API functions.
6. Create query keys.
7. Create TanStack Query hooks.
8. Create presentational components.
9. Create pages.
10. Add loading/error/empty states.
11. Add tests.
12. Verify no mock runtime data remains.
```

### 59.2 AI Prompt Constraint

Every AI implementation prompt should contain a rule equivalent to:

```text
Use real API responses only. Do not create mock business data, fixture products, fake orders, fake users, fake KPIs, or simulated API responses. If the backend endpoint does not exist, expose a documented unavailable state or add the required backend contract instead of inventing frontend data.
```

### 59.3 AI Backend Alignment

AI-generated frontend code must follow the uploaded backend architecture.

The frontend must align with:

- ASP.NET Core APIs.
- SQL Server persistence.
- Redis-backed services where exposed.
- Existing domain modules.
- Existing route groups.
- Existing order, payment, refund, review, and inventory concepts.

When a screen requires an endpoint not currently exposed by the backend, the implementation should identify the missing endpoint explicitly.

Do not hide missing backend capabilities behind fake frontend data.

### 59.4 AI Fixed Login Constraint

During Phase 1, AI may implement the fixed demo login.

AI must isolate it behind the authentication abstraction.

AI must not spread `if username === "admin"` checks throughout the application.

Role checks should be centralized in route guards and navigation helpers.

The eventual Keycloak migration should replace the adapter rather than rewrite feature components.

---

## 60. Phase 1 Implementation Checklist

Before moving to the homepage and product experience, confirm:

- [ ] React + TypeScript project is running.
- [ ] Tailwind CSS is configured.
- [ ] shadcn/ui primitives are installed.
- [ ] Lucide icons are available.
- [ ] TanStack Query is configured.
- [ ] React Router is configured.
- [ ] React Hook Form is configured.
- [ ] Zod is configured.
- [ ] Central API client exists.
- [ ] API errors are normalized.
- [ ] Loading state components exist.
- [ ] Empty state components exist.
- [ ] Error state components exist.
- [ ] Fixed demo login exists behind an auth abstraction.
- [ ] Logout clears the demo session.
- [ ] Customer and admin route guards exist for Phase 1.
- [ ] No runtime mock business data exists.
- [ ] No mock product arrays exist.
- [ ] No fake order arrays exist.
- [ ] No fake dashboard KPI values exist.
- [ ] No frontend-generated business IDs exist.
- [ ] Global header exists.
- [ ] Search bar exists.
- [ ] Category navigation reads from the API.
- [ ] Footer exists.
- [ ] Responsive breakpoints are implemented.
- [ ] Keyboard navigation is supported.
- [ ] Focus states are visible.
- [ ] Reduced-motion behavior is respected.

---

## 61. Phase 1 Completion Criteria

Part 1 is complete when the frontend foundation is capable of supporting the marketplace experience without architectural rework.

The application should be able to:

- Render the storefront shell.
- Render real categories from the API.
- Render real product results from the API.
- Navigate through a real product search flow.
- Maintain client-side server-state caching through TanStack Query.
- Show professional loading, error, empty, and not-found states.
- Support fixed demo login for customer and admin development flows.
- Protect customer and admin routes at the UI level for Phase 1.
- Keep business data sourced from the API.
- Remain structurally ready for future Keycloak replacement.

The next implementation part should build the complete homepage, category discovery experience, product listing page, product filters, sorting controls, product grid, and product details experience using the same design system.

---

# Part 2 Placeholder

The next section of this same document will define:

- Homepage UX.
- Hero search experience.
- Category discovery.
- Recently added products.
- Featured products when supported by the backend.
- Popular products when supported by real sales statistics.
- Recently viewed products.
- Product listing pages.
- Filtering.
- Sorting.
- Product cards.
- Product detail pages.
- Image galleries.
- Product information hierarchy.
- Reviews.
- Related products.
- Responsive behavior.
- Empty states.
- API integration details.
- Component hierarchy.
- Testing requirements.
