﻿namespace CrossCutting.Security.Authorization;

internal delegate IAuthorizationChecker AuthorizationCheckResolver(AuthorizationCheckStrategy checkStrategy);
