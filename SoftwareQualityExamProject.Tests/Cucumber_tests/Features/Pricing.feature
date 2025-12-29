Feature: Pricing calculation

  Scenario: TC1 No coupon, non-premium
    Given coupon is ""
    And basePrice is 100
    And premium is false
    When I call GET "/api/pricing/calculate"
    Then the response status should be 200
    And json bool "CouponApplied" should be false
    And json bool "PremiumApplied" should be false
    And json number "FinalPrice" should be 100
    And coupon service should not be called
    And pricing service should not be called

  Scenario: TC2 No coupon, premium
    Given coupon is ""
    And basePrice is 100
    And premium is true
    When I call GET "/api/pricing/calculate"
    Then the response status should be 200
    And json bool "CouponApplied" should be false
    And json bool "PremiumApplied" should be true
    And json number "FinalPrice" should be 90
    And coupon service should not be called
    And pricing service should not be called

  Scenario: TC3 Valid coupon, non-premium
    Given coupon is "SAVE10"
    And basePrice is 100
    And premium is false
    And coupon lookup returns a coupon
    And coupon is valid is true
    And pricing service returns 80
    When I call GET "/api/pricing/calculate"
    Then the response status should be 200
    And json bool "CouponApplied" should be true
    And json bool "PremiumApplied" should be false
    And json number "FinalPrice" should be 80
    And coupon service lookup should be called
    And coupon validation should be called
    And pricing service should be called once

  Scenario: TC4 Valid coupon, premium
    Given coupon is "SAVE10"
    And basePrice is 100
    And premium is true
    And coupon lookup returns a coupon
    And coupon is valid is true
    And pricing service returns 70
    When I call GET "/api/pricing/calculate"
    Then the response status should be 200
    And json bool "CouponApplied" should be true
    And json bool "PremiumApplied" should be true
    And json number "FinalPrice" should be 70
    And coupon service lookup should be called
    And coupon validation should be called
    And pricing service should be called once

  Scenario: TC5 Whitespace coupon behaves like no coupon
    Given coupon is "   "
    And basePrice is 100
    And premium is true
    When I call GET "/api/pricing/calculate"
    Then the response status should be 200
    And json bool "CouponApplied" should be false
    And json number "FinalPrice" should be 90
    And coupon service should not be called
    And pricing service should not be called

  Scenario: TC6 basePrice is 0 (edge case)
    Given coupon is ""
    And basePrice is 0
    And premium is true
    When I call GET "/api/pricing/calculate"
    Then the response status should be 200
    And json number "FinalPrice" should be 0
    And coupon service should not be called
    And pricing service should not be called

  Scenario: TC7 Very large basePrice (edge case)
    Given coupon is ""
    And basePrice is 1000000000
    And premium is true
    When I call GET "/api/pricing/calculate"
    Then the response status should be 200
    And json number "FinalPrice" should be 900000000

  Scenario: TC8 Negative basePrice passes (gap test)
    Given coupon is ""
    And basePrice is -100
    And premium is false
    When I call GET "/api/pricing/calculate"
    Then the response status should be 200
    And json number "FinalPrice" should be -100
    And coupon service should not be called
    And pricing service should not be called

  Scenario: TC9 Expired coupon via IsCouponValid=false
    Given coupon is "OLD"
    And basePrice is 100
    And premium is true
    And coupon lookup returns a coupon
    And coupon is valid is false
    When I call GET "/api/pricing/calculate"
    Then the response status should be 400
    And the response body should contain "Coupon is expired"
    And pricing service should not be called
    And coupon service lookup should be called
    And coupon validation should be called

  Scenario: TC10 Expired coupon via ExpiredCouponException (inconsistency test)
    Given coupon is "OLD"
    And basePrice is 100
    And premium is true
    And coupon lookup returns a coupon
    And coupon validation throws ExpiredCouponException with message "Expired"
    When I call GET "/api/pricing/calculate"
    Then the response status should be 400
    And the response body should contain "Expired"
    And coupon service lookup should be called
    And coupon validation should be called

  Scenario: TC11 Coupon not found
    Given coupon is "NOPE"
    And basePrice is 100
    And premium is false
    And coupon lookup throws NotFoundException with message "Coupon not found"
    When I call GET "/api/pricing/calculate"
    Then the response status should be 404
    And the response body should contain "Coupon not found"
    And coupon service lookup should be called

  Scenario: TC12 Unexpected exception -> 500 with generic message
    Given coupon is "SAVE10"
    And basePrice is 100
    And premium is false
    And coupon lookup returns a coupon
    And coupon is valid is true
    And pricing service throws an unexpected exception
    When I call GET "/api/pricing/calculate"
    Then the response status should be 500
    And the response body should contain "Unexpected error occurred"

  Scenario: TC13 JsonErrorException -> 500 with exception message
    Given coupon is "SAVE10"
    And basePrice is 100
    And premium is false
    And coupon lookup returns a coupon
    And coupon is valid is true
    And pricing service throws JsonErrorException with message "Bad JSON"
    When I call GET "/api/pricing/calculate"
    Then the response status should be 500
    And the response body should contain "Bad JSON"
